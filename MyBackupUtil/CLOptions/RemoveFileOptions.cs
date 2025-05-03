using CommandLine;

namespace MyBackupUtil.CLOptions;

[Verb("remove-file", HelpText = "Removes a file from the backup configuration")]
internal class RemoveFileOptions : BasePathOptions { }