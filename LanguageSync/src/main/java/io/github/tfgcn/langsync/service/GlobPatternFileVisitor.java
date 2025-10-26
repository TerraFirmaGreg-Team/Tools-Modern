package io.github.tfgcn.langsync.service;

import lombok.Getter;
import lombok.extern.slf4j.Slf4j;

import java.io.IOException;
import java.nio.file.*;
import java.nio.file.attribute.BasicFileAttributes;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

/**
 * desc:
 *
 * @author yanmaoyuan
 */
@Slf4j
public class GlobPatternFileVisitor extends SimpleFileVisitor<Path> {

    private final Path rootDir;
    private final PathMatcher matcher;
    private final List<PathMatcher> ignoreMatchers;
    @Getter
    private final List<Path> result;

    public GlobPatternFileVisitor(Path rootDir, String sourcePattern, List<String> ignores) {
        this.rootDir = rootDir;
        this.result = new ArrayList<>();

        this.matcher = FileSystems.getDefault().getPathMatcher("glob:" + sourcePattern);

        if (ignores == null || ignores.isEmpty()) {
            this.ignoreMatchers = Collections.emptyList();
        } else {
            this.ignoreMatchers = new ArrayList<>(ignores.size());
            for (String ignore : ignores) {
                // 支持glob模式的忽略规则，如**/.git/**、*.tmp等
                this.ignoreMatchers.add(FileSystems.getDefault().getPathMatcher("glob:" + ignore));
            }
        }
    }

    @Override
    public FileVisitResult preVisitDirectory(Path dir, BasicFileAttributes attrs) {
        Path relativeDir = rootDir.relativize(dir);
        for (PathMatcher ignore : ignoreMatchers) {
            if (ignore.matches(relativeDir)) {
                return FileVisitResult.SKIP_SUBTREE;
            }
        }
        return FileVisitResult.CONTINUE;
    }

    @Override
    public FileVisitResult visitFile(Path file, BasicFileAttributes attrs) {
        // 获取相对于根目录的路径用于匹配
        Path relativePath = rootDir.relativize(file);

        // 检查文件是否需要被忽略
        for (PathMatcher ignore : ignoreMatchers) {
            if (ignore.matches(relativePath)) {
                return FileVisitResult.CONTINUE;
            }
        }

        // 检查文件是否匹配目标模式
        if (matcher.matches(relativePath)) {
            result.add(file);
        }
        return FileVisitResult.CONTINUE;
    }

    @Override
    public FileVisitResult visitFileFailed(Path file, IOException exc) throws IOException {
        log.error("访问文件失败: {}", file, exc);
        throw exc;
    }
}
