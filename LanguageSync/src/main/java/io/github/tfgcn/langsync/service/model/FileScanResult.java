package io.github.tfgcn.langsync.service.model;

import io.github.tfgcn.langsync.Constants;
import lombok.Data;

/**
 * desc: 文件扫描结果
 *
 * @author yanmaoyuan
 */
@Data
public class FileScanResult {
    private String sourceFilePath;// relative path of workspace
    private String translationFilePath;// relative path of workspace

    public String getSourceFileFolder() {
        if (sourceFilePath == null) {
            return "";
        }
        int index = sourceFilePath.lastIndexOf(Constants.SEPARATOR);
        return index > -1 ? sourceFilePath.substring(0, index) : "";
    }

    public String getTranslationFileFolder() {
        if (translationFilePath == null) {
            return "";
        }
        int index = translationFilePath.lastIndexOf(Constants.SEPARATOR);
        return index > -1 ? translationFilePath.substring(0, index) : "";
    }
}