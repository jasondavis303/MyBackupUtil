using CommandLine;
using System;
using System.IO;

namespace MyBackupUtil.CLOptions;

internal abstract class BaseOptions
{
    [Option('c', "config", Required = false, HelpText = "Override the default config file location")]
    public FileInfo Config { get; set; } = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mybackuputil.json"));
}
