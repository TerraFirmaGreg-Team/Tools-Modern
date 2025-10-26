# Language Sync Tool

The main objective is:

- Add entries from en_us to the directory of a specified language (e.g. `ru_ru`).
- Only new entries will be appended, while existing ones will remain intact.

## Usage

Upgrade language entries:

```shell
#!/bin/sh
java -jar langsync-*.jar upgrade -w E:\repo -l ru_ru
```

Assembly translations, and write them to specified output dir.

```shell
java -jar langsync-*.jar assembly -w E:\repo -l zh_cn -o E:\HMCL-3.6.12\.minecraft\versions\TerraFirmaGreg-Modern-0.11.5
```

## Parameters

```
-w --workspace: The folder where you put Tools-Modern in.
-l --language: The target language you want to upgrade or assembly.
-o --output: The folder you want to write in.
```