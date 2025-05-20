using CommandLine;
using MyBackupUtil.CLOptions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MyBackupUtil;

internal class Program
{
    static async Task Main(string[] args)
    {
#if DEBUG
        string configFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "MyBackupUtil.json");
        //args = ["run"];
        //args = ["rclone", "-r", "MyRemote:backups/test", "-b", "MyRemote:backups/test.bak", "-f", "\"--tpslimit 5 --fast-list\""];
        //args = ["add-file","-p", configFile, "-f"];
        //args = ["remove-file", "-p", configFile];
        //args = ["add-directory", "-p", Directory.GetCurrentDirectory(), "-f", "-i", "*.exe"];
        //args = ["remove-directory", "-p", Directory.GetCurrentDirectory()];
        args = ["print"];
#endif
        try
        {
            await Parser.Default.ParseArguments<
                    RunOptions, 
                    AddDirectoryOptions, 
                    RemoveDirectoryOptions, 
                    AddFileOptions, 
                    RemoveFileOptions, 
                    SetRcloneOptions, 
                    PrintOptions,
                    AddConfigOptions,
                    RemoveConfigOptions,
                    UpdateOptions
                 >(args)
                .WithParsed<AddDirectoryOptions>(ConfigEditor.AddDirectory)
                .WithParsed<RemoveDirectoryOptions>(ConfigEditor.RemoveDirectory)
                .WithParsed<AddFileOptions>(ConfigEditor.AddFile)
                .WithParsed<RemoveFileOptions>(ConfigEditor.RemoveFile)
                .WithParsed<SetRcloneOptions>(ConfigEditor.SetRclone)
                .WithParsed<PrintOptions>(ConfigEditor.Print)
                .WithParsed<AddConfigOptions>(ConfigEditor.IncludeConfig)
                .WithParsed<RemoveConfigOptions>(ConfigEditor.ExcludeConfig)
                .WithParsed<RunOptions>(Runner.RunBackup)
                .WithParsedAsync<UpdateOptions>(AppUpdater.Update);
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
