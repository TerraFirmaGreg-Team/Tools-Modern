package io.github.tfgcn.langsync.service;

import io.github.tfgcn.langsync.service.model.FileScanRequest;
import io.github.tfgcn.langsync.service.model.FileScanResult;
import lombok.extern.slf4j.Slf4j;
import org.apache.commons.io.FileUtils;

import java.io.File;
import java.io.IOException;
import java.nio.file.*;
import java.util.ArrayList;
import java.util.List;

/**
 * File scan service
 * <p>This service is used to find files which matches the glob pattern.</p>
 *
 * @author yanmaoyuan
 */
@Slf4j
public class FileScanService {

    /**
     * Scan source language files, and mapping them to specified language files.
     *
     * @param request
     * @return
     * @throws IOException
     */
    public List<FileScanResult> scanAndMapFiles(FileScanRequest request) throws IOException {
        List<FileScanResult> results = new ArrayList<>();

        File workspace = new File(request.getWorkspace());
        if (!FileUtils.isDirectory(workspace)) {
            throw new IllegalArgumentException("Workspace must be a folder");
        }

        Path workspacePath = Paths.get(workspace.getCanonicalPath());

        // remove the very first "/", make sure the input path is relative.
        String sourceFilePattern;
        if (request.getSourceFilePattern().startsWith("/")) {
            sourceFilePattern = request.getSourceFilePattern().substring(1);
        } else {
            sourceFilePattern = request.getSourceFilePattern();
        }

        String translationFilePattern;
        if (request.getTranslationFilePattern().startsWith("/")) {
            translationFilePattern = request.getTranslationFilePattern().substring(1);
        } else {
            translationFilePattern = request.getTranslationFilePattern();
        }

        // Find all matching files, exclude the ignored ones.
        List<Path> matchedFiles = findFiles(workspacePath, sourceFilePattern, request.getIgnores());

        // Generate path mapping:
        // tfg/en_us/items.json -> tfg/zh_cn/items.json
        for (Path file : matchedFiles) {
            Path targetPath = generateTargetPath(
                    workspacePath,
                    file,
                    request.getSrcLang(),
                    request.getDestLang(),
                    translationFilePattern
            );

            String sourceRelativePath = workspacePath.relativize(file).toString().replace("\\", "/");
            String targetRelativePath = workspacePath.relativize(targetPath).toString().replace("\\", "/");

            FileScanResult result = new FileScanResult();
            result.setSourceFilePath(sourceRelativePath);
            result.setTranslationFilePath(targetRelativePath);
            results.add(result);
        }

        return results;
    }

    /**
     * Find all the exist files in specified language
     *
     * @param request
     * @return
     * @throws IOException
     */
    public List<String> scanExistFiles(FileScanRequest request) throws IOException {
        List<String> results = new ArrayList<>();

        File workspace = new File(request.getWorkspace());
        if (!FileUtils.isDirectory(workspace)) {
            throw new IllegalArgumentException("Workspace must be a folder");
        }

        Path workspacePath = Paths.get(workspace.getCanonicalPath());

        // remove the very first "/", make sure the input path is relative.
        String sourceFilePattern;
        if (request.getSourceFilePattern().startsWith("/")) {
            sourceFilePattern = request.getSourceFilePattern().substring(1);
        } else {
            sourceFilePattern = request.getSourceFilePattern();
        }

        // Now we are finding those exist files in specified language, so replace the pattern language.
        String existFilePattern = sourceFilePattern.replace(request.getSrcLang(), request.getDestLang());

        List<Path> matchedFiles = findFiles(workspacePath, existFilePattern, request.getIgnores());

        for (Path file : matchedFiles) {
            // Convert to relative path
            String existRelativePath = workspacePath.relativize(file).toString().replace("\\", "/");
            results.add(existRelativePath);
        }

        return results;
    }

    private List<Path> findFiles(Path baseDir, String pattern, List<String> ignores) throws IOException {
        String globPattern = pattern;
        Path rootDir;

        // Get the rootDir, before the first "*"
        int globStart = pattern.indexOf('*');
        int globQuestion = pattern.indexOf('?');
        int firstWildcard = Integer.MAX_VALUE;

        if (globStart != -1) firstWildcard = globStart;
        if (globQuestion != -1) firstWildcard = Math.min(firstWildcard, globQuestion);

        if (firstWildcard != Integer.MAX_VALUE) {
            String rootDirStr = pattern.substring(0, firstWildcard);
            rootDirStr = rootDirStr.substring(0, rootDirStr.lastIndexOf('/') + 1);
            rootDir = baseDir.resolve(rootDirStr).normalize();
            globPattern = pattern.substring(firstWildcard);
        } else {
            // use baseDir instead
            rootDir = baseDir;
        }

        GlobPatternFileVisitor visitor = new GlobPatternFileVisitor(rootDir, globPattern, ignores);

        Files.walkFileTree(rootDir, visitor);

        return visitor.getResult();
    }

    private Path generateTargetPath(
            Path workspacePath,
            Path sourceFile,
            String sourceLanguage,
            String targetLanguage,
            String translationPattern) {

        // Get relativePath to the workspace
        Path relativeToWorkspace = workspacePath.relativize(sourceFile);
        List<String> pathSegments = new ArrayList<>();
        relativeToWorkspace.forEach(segment -> pathSegments.add(segment.toString()));
        int segmentCount = pathSegments.size();

        // %original_path_pre%
        String originalPathPre;
        // %original_path%
        String originalPath;
        // %original_file_name%
        String originalFileName = pathSegments.get(segmentCount - 1);

        // Where is the source "language"
        int langIndex = getSrcLangIndex(sourceLanguage, pathSegments, originalFileName);
        if (langIndex > -1) {
            List<String> preSegments = pathSegments.subList(0, langIndex);
            originalPathPre = String.join("/", preSegments);
        } else {
            originalPathPre = "";// Not found
        }

        // Note, maybe the "language" is in the file name. (e.g. en_us.json) That means `postSegments` maybe not exist.
        if (langIndex < segmentCount - 1) {
            List<String> postSegments = pathSegments.subList(langIndex + 1, segmentCount - 1);
            originalPath = String.join("/", postSegments);
        } else {
            originalPath = "";
        }

        // Replace all the placeholders
        String targetPathStr = translationPattern
                .replace("%language%", targetLanguage)
                .replace("%original_path_pre%", originalPathPre)
                .replace("%original_path%", originalPath)
                .replace("%original_file_name%", originalFileName);

        return workspacePath.resolve(targetPathStr);
    }

    /**
     * GEt string index of the source language.
     * @param sourceLanguage The source language, e.g. en_us
     * @param pathSegments The path segments
     * @param originalFileName The file name
     * @return The index of source language, or else return -1。
     */
    private int getSrcLangIndex(String sourceLanguage, List<String> pathSegments, String originalFileName) {
        int langIndex = -1;
        for (int i = 0; i < pathSegments.size(); i++) {
            if (pathSegments.get(i).equals(sourceLanguage)) {
                langIndex = i;
                break;
            }
        }

        if (langIndex == -1) {
            // Maybe it's in the file name. e.g. en_us.json
            if (originalFileName.startsWith(sourceLanguage + ".")) {
                langIndex = pathSegments.size() - 1;
            } else {
                log.debug("Source language not found in path: {}", sourceLanguage);
            }
        }
        return langIndex;
    }
}
