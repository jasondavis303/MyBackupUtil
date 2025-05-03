using CommandLine;

namespace MyBackupUtil.CLOptions;

internal abstract class BasePathOptions : BaseOptions
{
    [Option('p', "path", Required = true, HelpText = "Path to file or directory")]
    public required string Path { get; set; }
}
