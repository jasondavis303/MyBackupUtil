using CommandLine;
using System.IO;

namespace MyBackupUtil.CLOptions;

internal abstract class BaseOptions
{
    [Option('c', "config", Required = true, HelpText = "Config file location")]
    public required FileInfo Config { get; set; }
}
