package io.github.tfgcn.langsync.service.model;

import lombok.Data;

import java.util.List;

/**
 * desc: 文件扫描请求
 *
 * @author yanmaoyuan
 */
@Data
public class FileScanRequest {
    private String workspace;// 工作空间，例如 D:/workspace。返回结果是相对于工作空间路径的相对路径。
    private String sourceFilePattern;// 源文件匹配模式，例如 test/**/%src_lang%/**.json
    private String translationFilePattern;// 映射译文路径模式，例如 test/%lang%/%original_path%/%original_file_name%
    private String srcLang;// 源文件语言，例如: en_us，记录为变量：%src_lang%
    private String destLang;// 译文语言，例如：zh_us，记录为变量：%lang%
    private List<String> ignores;// 忽略文件
}
