using CommandLine;
using System.Collections.Generic;

namespace MyBackupUtil.CLOptions;

[Verb("rclone", HelpText = "Configure options for Rclone")]
internal class SetRcloneOptions : BaseOptions
{
    [Option('r', "remote", Required = true, HelpText = "Remote path to send files to")]
    public required string Remote { get; set; }

    [Option('b', "backup", Required = false, HelpText = "Remote path to save old versions of files to")]
    public string? Backup { get; set; }

    [Option('f', "flags", Required = false, HelpText = "Extra flags for Rclone, surrounded by quotes E.g. \"-v --tpslimit 10\"")]
    public string? Flags { get; set; }
}
