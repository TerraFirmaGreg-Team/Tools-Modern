package io.github.tfgcn.transync.service;
import io.github.tfgcn.langsync.service.FileScanService;
import io.github.tfgcn.langsync.service.model.FileScanRequest;
import io.github.tfgcn.langsync.service.model.FileScanResult;
import lombok.Builder;
import lombok.Data;
import lombok.extern.slf4j.Slf4j;
import org.apache.commons.io.FileUtils;
import org.junit.jupiter.api.AfterAll;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.Test;

import java.io.File;
import java.io.IOException;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

@Slf4j
class FileScanServiceTest {

    // ========================= 测试路径常量定义 =========================
    private static final String SRC_LANG = "en_us";
    private static final String DEST_LANG = "zh_cn";

    // 模式1：语言文件夹下直接有文件
    private static final String PATTERN1_FILE1 = "test/en_us/blocks.json";
    private static final String PATTERN1_FILE2 = "test/en_us/items.json";
    private static final String PATTERN1_FILE1_TARGET = "test/zh_cn/blocks.json";
    private static final String PATTERN1_FILE2_TARGET = "test/zh_cn/items.json";

    // 模式2：语言文件夹下有子文件夹
    private static final String PATTERN2_FILE1 = "test/en_us/Quests/chapter1.json";
    private static final String PATTERN2_FILE2 = "test/en_us/Quests/chapter2/chapter2.json";
    private static final String PATTERN2_FILE3 = "test/en_us/Quests/chapter3/chapter3/chapter3.json";
    private static final String PATTERN2_FILE1_TARGET = "test/zh_cn/Quests/chapter1.json";
    private static final String PATTERN2_FILE2_TARGET = "test/zh_cn/Quests/chapter2/chapter2.json";
    private static final String PATTERN2_FILE3_TARGET = "test/zh_cn/Quests/chapter3/chapter3/chapter3.json";

    // 模式3：语言文件夹前有上级目录（无深层子目录）
    private static final String PATTERN3_FILE1 = "test/tfg/en_us/lang.json";
    private static final String PATTERN3_FILE2 = "test/ae2/en_us/lang.json";
    private static final String PATTERN3_FILE1_TARGET = "test/tfg/zh_cn/lang.json";
    private static final String PATTERN3_FILE2_TARGET = "test/ae2/zh_cn/lang.json";

    // 模式4：语言文件夹前有上级目录且有深层子目录
    private static final String PATTERN4_FILE1 = "test/tfg/en_us/Quests/chapter.json";
    private static final String PATTERN4_FILE2 = "test/ae2/en_us/Quests/chapter.json";
    private static final String PATTERN4_FILE1_TARGET = "test/tfg/zh_cn/Quests/chapter.json";
    private static final String PATTERN4_FILE2_TARGET = "test/ae2/zh_cn/Quests/chapter.json";

    // 模式5：文件名为语言类型
    private static final String PATTERN5_FILE1 = "test/tfc/en_us.json";
    private static final String PATTERN5_FILE2 = "test/create/en_us.json";
    private static final String PATTERN5_FILE1_TARGET = "test/tfc/zh_cn.json";
    private static final String PATTERN5_FILE2_TARGET = "test/create/zh_cn.json";

    // 所有测试文件集合
    private static final List<String> ALL_TEST_FILES = List.of(
            PATTERN1_FILE1, PATTERN1_FILE2,
            PATTERN2_FILE1, PATTERN2_FILE2, PATTERN2_FILE3,
            PATTERN3_FILE1, PATTERN3_FILE2,
            PATTERN4_FILE1, PATTERN4_FILE2, PATTERN5_FILE1, PATTERN5_FILE2
    );


    // ========================= 测试初始化与清理 =========================
    @BeforeAll
    static void createTestFiles() throws IOException {
        for (String filePath : ALL_TEST_FILES) {
            createFile(filePath);
        }
    }

    @AfterAll
    static void cleanTestFiles() throws IOException {
        File testDir = new File("test");
        if (testDir.exists()) {
            FileUtils.deleteDirectory(testDir);
        }
    }

    static void createFile(String path) throws IOException {
        File file = new File(path);
        if (!file.exists()) {
            FileUtils.createParentDirectories(file);
            if (!file.createNewFile()) {
                log.error("创建文件失败: {}", path);
            }
        }
    }


    // ========================= 测试参数封装 =========================
    /**
     * 测试用例参数封装类，统一管理每个模式的测试配置
     */
    @Data
    @Builder
    static class ScanTestParam {
        String testName; // 测试名称（用于日志区分）
        String sourceFilePattern; // 源文件匹配模式
        String translationFilePattern; // 译文路径模式
        int expectedFileCount; // 预期扫描文件数量
        Map<String, String> sourceTargetMap; // 源文件路径 -> 预期译文路径的映射
    }


    // ========================= 统一测试模板方法 =========================
    /**
     * 通用测试模板，接收参数并执行扫描+验证流程
     */
    private void runScanTest(ScanTestParam param) throws IOException {
        log.info("开始执行测试: {}", param.getTestName());

        // 1. 构建请求
        FileScanRequest request = new FileScanRequest();
        request.setWorkspace(".");
        request.setSourceFilePattern(param.getSourceFilePattern());
        request.setTranslationFilePattern(param.getTranslationFilePattern());
        request.setSrcLang(SRC_LANG);
        request.setDestLang(DEST_LANG);

        // 2. 执行扫描
        FileScanService service = new FileScanService();
        List<FileScanResult> results = service.scanAndMapFiles(request);

        logResult(results);// 日志输出

        // 3. 转换结果为Map
        Map<String, String> resultMap = results.stream()
                .collect(Collectors.toMap(
                        FileScanResult::getSourceFilePath,
                        FileScanResult::getTranslationFilePath
                ));

        // 4. 验证文件数量
        assertEquals(param.getExpectedFileCount(), results.size(),
                param.getTestName() + "：预期扫描到" + param.getExpectedFileCount() + "个文件");

        // 5. 验证每个文件的映射路径
        param.getSourceTargetMap().forEach((sourcePath, expectedTarget) ->
                assertMapping(resultMap, sourcePath, expectedTarget)
        );

        log.info("测试通过: {}\n", param.getTestName());
    }


    // ========================= 各模式测试用例（参数化调用） =========================
    @Test
    void testPattern1ScanFile() throws IOException {
        // 构建模式1的测试参数
        ScanTestParam param = ScanTestParam.builder()
                .testName("模式1：语言文件夹下直接有文件")
                .sourceFilePattern("test/en_us/*.json")
                .translationFilePattern("test/%language%/%original_file_name%")
                .expectedFileCount(2)
                .sourceTargetMap(Map.of(
                        PATTERN1_FILE1, PATTERN1_FILE1_TARGET,
                        PATTERN1_FILE2, PATTERN1_FILE2_TARGET
                ))
                .build();

        // 调用通用模板执行测试
        runScanTest(param);
    }

    @Test
    void testPattern2ScanAll() throws IOException {
        ScanTestParam param = ScanTestParam.builder()
                .testName("模式2：扫描所有文件（含子文件夹）")
                .sourceFilePattern("test/en_us/**.json")
                .translationFilePattern("test/%language%/%original_path%/%original_file_name%")
                .expectedFileCount(5) // 模式1的2个+模式2的3个
                .sourceTargetMap(Map.of(
                        PATTERN1_FILE1, PATTERN1_FILE1_TARGET,
                        PATTERN1_FILE2, PATTERN1_FILE2_TARGET,
                        PATTERN2_FILE1, PATTERN2_FILE1_TARGET,
                        PATTERN2_FILE2, PATTERN2_FILE2_TARGET,
                        PATTERN2_FILE3, PATTERN2_FILE3_TARGET
                ))
                .build();

        runScanTest(param);
    }

    @Test
    void testPattern2OnlySubFolders() throws IOException {
        ScanTestParam param = ScanTestParam.builder()
                .testName("模式2：只扫描子文件夹，不含直接文件")
                .sourceFilePattern("test/en_us/**/*.json")
                .translationFilePattern("test/%language%/%original_path%/%original_file_name%")
                .expectedFileCount(3)
                .sourceTargetMap(Map.of(
                        PATTERN2_FILE1, PATTERN2_FILE1_TARGET,
                        PATTERN2_FILE2, PATTERN2_FILE2_TARGET,
                        PATTERN2_FILE3, PATTERN2_FILE3_TARGET
                ))
                .build();

        runScanTest(param);
    }

    @Test
    void testPattern3ScanFiles() throws IOException {
        ScanTestParam param = ScanTestParam.builder()
                .testName("模式3:语言文件夹前有上级目录")
                .sourceFilePattern("test/**/en_us/*.json")
                .translationFilePattern("%original_path_pre%/%language%/%original_file_name%")
                .expectedFileCount(2)
                .sourceTargetMap(Map.of(
                        PATTERN3_FILE1, PATTERN3_FILE1_TARGET,
                        PATTERN3_FILE2, PATTERN3_FILE2_TARGET
                ))
                .build();

        runScanTest(param);
    }

    @Test
    void testPattern4ScanAll() throws IOException {
        ScanTestParam param = ScanTestParam.builder()
                .testName("模式4:语言文件夹前有上级目录且有子目录")
                .sourceFilePattern("test/**/en_us/**.json")
                .translationFilePattern("%original_path_pre%/%language%/%original_path%/%original_file_name%")
                .expectedFileCount(4) // 模式3的2个+模式4的2个
                .sourceTargetMap(Map.of(
                        PATTERN3_FILE1, PATTERN3_FILE1_TARGET,
                        PATTERN3_FILE2, PATTERN3_FILE2_TARGET,
                        PATTERN4_FILE1, PATTERN4_FILE1_TARGET,
                        PATTERN4_FILE2, PATTERN4_FILE2_TARGET
                ))
                .build();

        runScanTest(param);
    }

    @Test
    void testPattern4OnlySubFolders() throws IOException {
        ScanTestParam param = ScanTestParam.builder()
                .testName("模式4：只扫描子文件夹，不含直接文件")
                .sourceFilePattern("test/**/en_us/**/*.json")
                .translationFilePattern("%original_path_pre%/%language%/%original_path%/%original_file_name%")
                .expectedFileCount(2)
                .sourceTargetMap(Map.of(
                        PATTERN4_FILE1, PATTERN4_FILE1_TARGET,
                        PATTERN4_FILE2, PATTERN4_FILE2_TARGET
                ))
                .build();

        runScanTest(param);
    }

    @Test
    void testPattern5ScanFiles() throws IOException {
        ScanTestParam param = ScanTestParam.builder()
                .testName("模式5: 文件名含有语言元素")
                .sourceFilePattern("test/**/en_us.json")
                .translationFilePattern("%original_path_pre%/%language%.json")
                .expectedFileCount(2)
                .sourceTargetMap(Map.of(
                        PATTERN5_FILE1, PATTERN5_FILE1_TARGET,
                        PATTERN5_FILE2, PATTERN5_FILE2_TARGET
                )).build();

        runScanTest(param);
    }


    // ========================= 辅助方法（不变） =========================
    private void assertMapping(Map<String, String> resultMap, String sourcePath, String expectedTargetPath) {
        assertTrue(resultMap.containsKey(sourcePath), "源文件[" + sourcePath + "]未被扫描到");
        String actualTargetPath = resultMap.get(sourcePath);
        assertEquals(
                expectedTargetPath,
                actualTargetPath,
                "文件[" + sourcePath + "]的译文路径映射错误（预期：" + expectedTargetPath + "，实际：" + actualTargetPath + "）"
        );
        log.debug("验证通过: {} -> {}", sourcePath, actualTargetPath);
    }

    private void logResult(List<FileScanResult> results) {
        for (FileScanResult result : results) {
            log.debug("{} -> {}", result.getSourceFilePath(), result.getTranslationFilePath());
        }
    }
}