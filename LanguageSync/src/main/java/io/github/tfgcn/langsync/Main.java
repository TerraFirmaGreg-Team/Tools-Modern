package io.github.tfgcn.langsync;

import com.google.gson.reflect.TypeToken;
import io.github.tfgcn.langsync.cmd.*;
import io.github.tfgcn.langsync.service.SyncService;
import lombok.extern.slf4j.Slf4j;

import java.io.IOException;
import java.lang.reflect.Type;
import java.util.Map;

import picocli.CommandLine;

@Slf4j
public class Main {

    public static void main(String[] args) throws IOException {
        CommandLine commandLine = new CommandLine(new MainCommand())
                .addSubcommand(new AssemblyCommand())
                .addSubcommand(new UpgradeCommand());
        commandLine.setExecutionStrategy(new CommandLine.RunLast());
        System.exit(commandLine.execute(args));
    }
}
