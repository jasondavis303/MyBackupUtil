using CommandLine;

namespace MyBackupUtil.CLOptions;

[Verb("include-config-in-backups", HelpText = "Adds the config file to the backups")]
internal class AddConfigOptions : BaseOptions { }