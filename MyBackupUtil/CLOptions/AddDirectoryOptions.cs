using CommandLine;
using System.Collections.Generic;

namespace MyBackupUtil.CLOptions;

[Verb("add-directory", HelpText = "Adds a directory to the backup configuration")]
internal class AddDirectoryOptions : BaseAddPathOptions
{
    [Option('i', "include", SetName = "include")]
    public IEnumerable<string> Includes { get; set; } = [];

    [Option('e', "exclude", SetName = "exclude")]
    public IEnumerable<string> Excludes { get; set; } = [];
}
