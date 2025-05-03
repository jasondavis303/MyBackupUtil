using CommandLine;

namespace MyBackupUtil.CLOptions;

[Verb("add-file", HelpText = "Adds a file to the backup configuration")]
internal class AddFileOptions : BaseAddPathOptions { }