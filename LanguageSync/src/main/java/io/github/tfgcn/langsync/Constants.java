package io.github.tfgcn.langsync;

public final class Constants {

    private Constants() {
    }

    public static final String MOD_LANG_REGEX = "^.*?Tools-Modern/LanguageMerger/LanguageFiles/([^/]+)/([^/]+)/.+$";

    public static final String SOURCE_PATTERN = "Tools-Modern/LanguageMerger/LanguageFiles/**/en_us/**.json";
    public static final String TRANSLATION_PATTERN = "%original_path_pre%/%language%/%original_path%/%original_file_name%";
    public static final String SRC_LANG = "en_us";
    public static final String[] IGNORES = {"tfg/**/ore_veins.json"};

    public static final String DEFAULT_WORKSPACE = "..";

    public static final String SEPARATOR = "/";

}