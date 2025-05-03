using CommandLine;

namespace MyBackupUtil.CLOptions;

internal abstract class BaseAddPathOptions : BasePathOptions
{
    [Option('f', "force", Required = false, HelpText = "Forces a backup (backup will fail if the file is missing)")]
    public bool ErrorIfMissing { get; set; }
}
