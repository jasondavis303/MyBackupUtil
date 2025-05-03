using CommandLine;

namespace MyBackupUtil.CLOptions;

[Verb("remove-directory", HelpText = "Removes a directory from the backup configuration")]
internal class RemoveDirectoryOptions : BasePathOptions { }
