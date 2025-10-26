package io.github.tfgcn.langsync.service;

import com.google.gson.reflect.TypeToken;
import io.github.tfgcn.langsync.Constants;
import io.github.tfgcn.langsync.service.model.*;
import io.github.tfgcn.langsync.utils.JsonUtils;
import lombok.extern.slf4j.Slf4j;

import org.apache.commons.io.FileUtils;

import java.io.File;
import java.io.IOException;
import java.lang.reflect.Type;
import java.util.*;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import static io.github.tfgcn.langsync.Constants.*;

@Slf4j
public class SyncService {

    private String workDir;

    private String lang;

    private Map<String, Map<String, String>> existFilesMap;
    private Map<String, Map<String, Map<String, String>>> modLangMap;

    private final Pattern regex = Pattern.compile(Constants.MOD_LANG_REGEX);

    private final FileScanService fileScanService;

    public SyncService(String lang) {
        this.workDir = DEFAULT_WORKSPACE;
        this.lang = lang;
        this.existFilesMap = Collections.emptyMap();
        this.modLangMap = Collections.emptyMap();

        this.fileScanService = new FileScanService();
    }

    public void setWorkspace(String workspace) throws IOException {
        File workspaceFolder = new File(workspace);
        if (!workspaceFolder.exists()) {
            log.warn("folder not found: {}", workspace);
            throw new IOException("Folder not exists:" + workspace);
        }
        if (!workspaceFolder.isDirectory()) {
            log.warn("not a directory: {}", workspace);
            throw new IOException("Invalid folder");
        }
        this.workDir = workspaceFolder.getCanonicalPath().replace("\\", SEPARATOR);
        log.info("set workdir to:{}", workDir);
    }

    public void scanExistFiles(FileScanRequest request) throws IOException {
        log.info("Scanning exist language files...");
        List<String> exists = fileScanService.scanExistFiles(request);
        exists.sort(Comparator.naturalOrder());
        this.existFilesMap = new HashMap<>();
        this.modLangMap = new HashMap<>();

        Type mapType = new TypeToken<Map<String, String>>() {}.getType();

        for (String item : exists) {
            Matcher matcher = regex.matcher(item);
            if (matcher.matches()) {
                String mod = matcher.group(1);
                String lang = matcher.group(2);
                // read it to cache
                try {
                    Map<String, String> map = JsonUtils.readFile(getAbsoluteFile(item), mapType);
                    existFilesMap.put(item, map);

                    Map<String, Map<String, String>> langMap = modLangMap.computeIfAbsent(mod, k -> new HashMap<>());
                    Map<String, String> modMap = langMap.computeIfAbsent(lang, k -> new HashMap<>());
                    modMap.putAll(map);
                } catch (Exception ex) {
                    log.error("read file failed: {}", item, ex);
                    throw ex;
                }
            }
        }
    }

    public List<FileScanResult> getSourceFiles(FileScanRequest request) {

        List<FileScanResult> fileList = new ArrayList<>(100);

        Set<String> distinct = new HashSet<>();
        try {
            List<FileScanResult> results = fileScanService.scanAndMapFiles(request);
            for (FileScanResult item : results) {
                if (!distinct.contains(item.getTranslationFilePath())) {
                    distinct.add(item.getTranslationFilePath());
                    fileList.add(item);
                } else {
                    log.debug("Duplicated file:{}", item.getTranslationFilePath());
                }
            }
        } catch (Exception ex) {
            log.error("Scan file failed, request:{}", request, ex);
        }
        fileList.sort(Comparator.comparing(FileScanResult::getSourceFilePath));
        return fileList;
    }

    public void assembly(String outputDir) throws IOException {
        String outDir = setOutDir(outputDir);

        FileScanRequest request = new FileScanRequest();
        request.setWorkspace(this.workDir);
        request.setSourceFilePattern(Constants.SOURCE_PATTERN);
        request.setTranslationFilePattern(Constants.TRANSLATION_PATTERN);
        request.setSrcLang(Constants.SRC_LANG);
        request.setDestLang(lang);

        scanExistFiles(request);

        Set<String> mods = modLangMap.keySet();

        for (String mod : mods) {
            Map<String, String> map = modLangMap.get(mod).get(lang);
            String fileName = String.format("kubejs/assets/%s/lang/%s.json", mod, lang);
            File file = new File(outDir + SEPARATOR + fileName);
            FileUtils.createParentDirectories(file);
            JsonUtils.writeFile(file, map);
            log.info("write file:{}", file);
        }
    }

    public String setOutDir(String outputDir) throws IOException {
        File outputFolder = new File(outputDir);
        if (outputFolder.exists() && !outputFolder.isDirectory()) {
            log.warn("Not a directory: {}", outputDir);
            throw new IOException("Invalid folder");
        }
        return outputFolder.getCanonicalPath().replace("\\", SEPARATOR);
    }

    public void upgrade() throws IOException {
        FileScanRequest request = new FileScanRequest();
        request.setWorkspace(this.workDir);
        request.setSourceFilePattern(Constants.SOURCE_PATTERN);
        request.setTranslationFilePattern(Constants.TRANSLATION_PATTERN);
        request.setSrcLang(Constants.SRC_LANG);
        request.setDestLang(lang);
        request.setIgnores(Arrays.asList(Constants.IGNORES));

        scanExistFiles(request);

        List<FileScanResult> fileList = getSourceFiles(request);

        log.info("Found files: {}", fileList.size());
        for (FileScanResult item : fileList) {

            Matcher matcher = regex.matcher(item.getTranslationFilePath());
            if (matcher.matches()) {
                String mod = matcher.group(1);
                String lang = matcher.group(2);
                log.info("mod:{}, lang:{}, file:{}", mod, lang, item);

                File file = getAbsoluteFile(item.getSourceFilePath());
                String translationFileFolder = item.getTranslationFileFolder();

                Map<String, String> existFile = existFilesMap.get(item.getTranslationFilePath());
                if (existFile == null) {
                    createFile(mod, lang, translationFileFolder, file);
                } else {
                    modifyFile(existFile, mod, translationFileFolder, file);
                }
            }
        }
    }

    public void createFile(String mod, String lang, String translationFileFolder, File sourceFile) throws IOException {
        log.info("[NEW] {}/{}", translationFileFolder, sourceFile.getName());
        Map<String, String> sourceMap = JsonUtils.readFile(sourceFile, Map.class);

        // maybe there are some translated contents
        Map<String, Map<String, String>> langMap = modLangMap.get(mod);
        if (langMap != null) {
            Map<String, String> existContents = langMap.get(lang);
            if (existContents != null) {
                for (Map.Entry<String, String> entry : sourceMap.entrySet()) {
                    String key = entry.getKey();
                    if (existContents.containsKey(key)) {
                        String existValue = existContents.get(key);
                        sourceMap.put(key, existValue);
                    }
                }
            }
        }

        File targetFile = getAbsoluteFile(translationFileFolder + Constants.SEPARATOR + sourceFile.getName());
        FileUtils.createParentDirectories(targetFile);
        JsonUtils.writeFile(targetFile, sourceMap);
    }

    public void modifyFile(Map<String, String> existFile, String mod, String translationFileFolder, File sourceFile) throws IOException {
        Map<String, String> sourceMap = JsonUtils.readFile(sourceFile, Map.class);

        boolean isUpdated = false;
        for (Map.Entry<String, String> entry : sourceMap.entrySet()) {
            String key = entry.getKey();
            String value = entry.getValue();
            if (!existFile.containsKey(key)) {
                log.info("[append] {}/{}:{}", mod, key, value);
                existFile.put(key, value);
                isUpdated = true;
            }
        }

        if (isUpdated) {
            File targetFile = getAbsoluteFile(translationFileFolder + Constants.SEPARATOR + sourceFile.getName());
            FileUtils.createParentDirectories(targetFile);
            JsonUtils.writeFile(targetFile, sourceMap);
            log.info("[Upgrade] {}/{}", translationFileFolder, sourceFile.getName());
        } else {
            log.info("[Not modified] {}/{}", translationFileFolder, sourceFile.getName());
        }
    }

    public File getAbsoluteFile(String relativePath) {
        return new File(workDir + SEPARATOR + relativePath);
    }

}
