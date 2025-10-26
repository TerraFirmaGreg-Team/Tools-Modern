package io.github.tfgcn.langsync.cmd;

import io.github.tfgcn.langsync.service.SyncService;
import lombok.extern.slf4j.Slf4j;
import picocli.CommandLine;

import java.util.concurrent.Callable;

@Slf4j
@CommandLine.Command(name = "assembly", mixinStandardHelpOptions = true,
        description ="Assembly translated language files, output to kubejs/assets.")
public class AssemblyCommand extends BaseCommand implements Callable<Integer> {

    @CommandLine.Option(names = {"-l", "--language"}, description = "The target language to assembly, e.g. zh_cn, ru_ru.", required = true)
    protected String lang;

    @CommandLine.Option(names = {"-o", "--output"}, description = "The output folder to store kubejs/assets")
    protected String outputDir;

    @Override
    public Integer call() throws Exception {

        SyncService app = new SyncService(lang);
        app.setWorkspace(workspace);
        app.assembly(outputDir);
        return 0;
    }

}
