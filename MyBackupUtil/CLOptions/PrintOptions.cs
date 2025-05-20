using CommandLine;

namespace MyBackupUtil.CLOptions;

[Verb("print", HelpText = "Print the config file")]
internal class PrintOptions : BaseOptions { }