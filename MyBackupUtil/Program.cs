using CommandLine;
using MyBackupUtil.CLOptions;
using System;
using System.IO;

namespace MyBackupUtil;

internal class Program
{
    static void Main(string[] args)
    {
#if DEBUG
        string configFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "MyBackupUtil.json");
        args = ["run", "-c", configFile];
        //args = ["rclone", "-c", configFile, "-r", "MyRemote:backups/test", "-b", "MyRemote:backups/test.bak", "-f", "\"--tpslimit 5 --fast-list\""];
        //args = ["add-file", "-c", configFile, "-p", configFile, "-f"];
        //args = ["remove-file", "-c", configFile, "-p", configFile];
        //args = ["add-directory", "-c", configFile, "-p", Directory.GetCurrentDirectory(), "-f", "-i", "*.exe"];
        //args = ["remove-directory", "-c", configFile, "-p", Directory.GetCurrentDirectory()];
#endif

        try
        {
            Parser.Default.ParseArguments<RunOptions, AddDirectoryOptions, RemoveDirectoryOptions, AddFileOptions, RemoveFileOptions, SetRcloneOptions>(args)
                .WithParsed<AddDirectoryOptions>(ConfigEditor.AddDirectory)
                .WithParsed<RemoveDirectoryOptions>(ConfigEditor.RemoveDirectory)
                .WithParsed<AddFileOptions>(ConfigEditor.AddFile)
                .WithParsed<RemoveFileOptions>(ConfigEditor.RemoveFile)
                .WithParsed<SetRcloneOptions>(ConfigEditor.SetRclone)
                .WithParsed<RunOptions>(Runner.RunBackup);
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.ToString());
            Console.ResetColor();
            Console.WriteLine();
        }
    }



}
