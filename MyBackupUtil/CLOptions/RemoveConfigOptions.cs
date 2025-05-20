using CommandLine;

namespace MyBackupUtil.CLOptions;

[Verb("remove-config-from-backups", HelpText = "Removes the config file from the backups")]
internal class RemoveConfigOptions : BaseOptions { }