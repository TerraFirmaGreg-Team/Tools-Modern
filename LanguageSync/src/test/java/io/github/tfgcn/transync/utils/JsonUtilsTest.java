package io.github.tfgcn.transync.utils;

import io.github.tfgcn.langsync.utils.JsonUtils;
import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Test;

import java.util.LinkedHashMap;
import java.util.Map;

/**
 * desc:
 *
 * @author yanmaoyuan
 */
class JsonUtilsTest {

    @Test
    void toJsonTest() {
        Map<String, String> map = new LinkedHashMap<>();
        map.put("key", "value");
        map.put("foo", "bar");
        String json = JsonUtils.toJson(map);

        String expected = "{\n    \"key\": \"value\",\n    \"foo\": \"bar\"\n}";
        Assertions.assertEquals(expected, json);
    }
}
