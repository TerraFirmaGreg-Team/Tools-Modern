package io.github.tfgcn.langsync.utils;

import com.google.gson.*;

import java.io.*;
import java.lang.reflect.Type;
import java.nio.charset.StandardCharsets;

/**
 * desc:
 *
 * @author yanmaoyuan
 */
public final class JsonUtils {

    public static final Gson GSON;

    static {
        GSON = new GsonBuilder()
                .setFormattingStyle(FormattingStyle.PRETTY
                        .withIndent("    ")
                        .withNewline(System.lineSeparator()))
                .setObjectToNumberStrategy(ToNumberPolicy.LAZILY_PARSED_NUMBER)
                .serializeNulls()
                .disableHtmlEscaping()
                .create();
    }

    private JsonUtils() {}

    public static String toJson(Object obj) {
        return GSON.toJson(obj);
    }

    public static <T> T fromJson(String json, Class<T> clazz) {
        return GSON.fromJson(json, clazz);
    }

    public static <T> void writeFile(File file, T obj) throws IOException {
        try (Writer writer = new OutputStreamWriter(new FileOutputStream(file), StandardCharsets.UTF_8)) {
            GSON.toJson(obj, writer);
        }
    }

    public static <T> T readFile(File file, Type type) throws IOException {
        try (Reader reader = new InputStreamReader(new FileInputStream(file), StandardCharsets.UTF_8)) {
            return GSON.fromJson(reader, type);
        }
    }
}
