package io.github.tfgcn.langsync.cmd;

import lombok.extern.slf4j.Slf4j;
import picocli.CommandLine;

import java.io.IOException;

/**
 * @author yanmaoyuan
 */
@Slf4j
class BaseCommand {

    @CommandLine.Option(names = {"-w", "--workspace"}, required = true, description = "The folder where stored the Tools-Modern.")
    protected String workspace;
}
