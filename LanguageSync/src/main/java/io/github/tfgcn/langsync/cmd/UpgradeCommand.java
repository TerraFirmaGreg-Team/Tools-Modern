package io.github.tfgcn.langsync.cmd;

import io.github.tfgcn.langsync.service.SyncService;
import lombok.extern.slf4j.Slf4j;
import picocli.CommandLine;

import java.util.concurrent.Callable;

@Slf4j
@CommandLine.Command(name = "upgrade", mixinStandardHelpOptions = true,
        description ="Update translation language entries from en_us language files.")
public class UpgradeCommand extends BaseCommand implements Callable<Integer> {

    @CommandLine.Option(names = {"-l", "--language"}, description = "The target language to upgrade, e.g. zh_cn, ru_ru.", required = true)
    protected String lang;

    @Override
    public Integer call() throws Exception {

        SyncService app = new SyncService(lang);
        app.setWorkspace(workspace);
        app.upgrade();
        return 0;
    }

}
