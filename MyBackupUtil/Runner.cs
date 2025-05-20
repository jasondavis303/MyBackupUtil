using ConsoleAppFramework;
using MyBackupUtil.ConfigOptions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MyBackupUtil;

[RegisterCommands]
internal class Runner
{
    /// <summary>
    /// Runs the backup
    /// </summary>
    /// <param name="configFile">-c, Config file</param>
    public void Run(string? configFile = null)
    {
        if (string.IsNullOrWhiteSpace(configFile))
            configFile = Config.DefaultConfigFile;

        Config config = Config.Load(configFile);

        Console.WriteLine("Starting backup");
        Console.WriteLine();

        DirectoryInfo tmpDir = new DirectoryInfo(Path.GetTempPath()).SubDirectory(Guid.NewGuid().ToString("N"));

        try
        {
            //Use rclone to copy files/folders, because it keeps timestamps

            string args;


            //Add config file if desired
            if (config.IncludeConfigFileInBackup)
                if (!config.Files.Contains(configFile))
                    config.Files.Add(configFile);


            //Copy files
            foreach (string file in config.Files)
            {
                FileInfo fileInfo = new(file);
                if (fileInfo.Exists)
                {
                    string newFile = Path.Combine(tmpDir.FullName, fileInfo.FullName.Replace(":", null).Trim(['/', '\\']));
                    args = $"copyto \"{fileInfo.FullName}\" \"{newFile}\" -v";
                    RunRclone(args);
                    Console.WriteLine();
                }
            }


            //Copy dirs
            foreach (DirectoryBackup db in config.Directories)
            {
                DirectoryInfo dirInfo = new(db.Directory);
                if (dirInfo.Exists)
                {
                    string newDir = Path.Combine(tmpDir.FullName, dirInfo.FullName.Replace(":", null).Trim(['/', '\\']));

                    string includes = string.Join(" ", db.RcloneIncludes.Select(_ => "--include " + _));
                    string excludes = string.Join(" ", db.RcloneExcludes.Select(_ => "--exclude " + _));

                    args = $"copy \"{dirInfo.FullName}\" \"{newDir}\" --skip-links -v {includes} {excludes}".Trim();
                    RunRclone(args);
                    Console.WriteLine();
                }
            }


            //Backup
            Console.WriteLine("Syncing files");
            tmpDir.DeleteEmptyChildren();


            args = $"sync \"{tmpDir.FullName}\" \"{config.RcloneRemote}\"";
            if (!string.IsNullOrWhiteSpace(config.RcloneBackup))
                args += $" --backup-dir \"{config.RcloneBackup}\" --suffix {DateTime.UtcNow:_yyyyMMddhhmmss} --suffix-keep-extension";

            string flags = (config.RcloneFlags + string.Empty).Trim();
            if (!(flags.StartsWith("-v ") || flags.Contains(" -v ") || flags.Equals("-v") || flags.EndsWith(" -v")))
                flags += " -v";
            args += " " + flags.Trim();

            args = args.Trim();
            Console.WriteLine();
            RunRclone(args);
        }
        finally
        {
            Console.WriteLine("Cleaning up");
            tmpDir.TryDelete(true);
        }

        Console.WriteLine("Backup complete");
    }



    static void RunRclone(string args)
    {
        try
        {
            Console.WriteLine("rclone " + args);
            Console.ForegroundColor = ConsoleColor.DarkGray;


            using var proc = new Process
            {
                StartInfo = new ProcessStartInfo("rclone", args)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    ErrorDialog = false,
                    RedirectStandardOutput = true,
                    StandardOutputEncoding = System.Text.Encoding.UTF8,
                    RedirectStandardError = true,
                    StandardErrorEncoding = System.Text.Encoding.UTF8,
                },
                EnableRaisingEvents = true
            };
            proc.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
            proc.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);


            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();
            proc.WaitForExit();

            if (proc.ExitCode != 0)
            {
                string msg = proc.ExitCode switch
                {
                    1 => "Syntax or usage error",
                    2 => "Error not otherwise categorised",
                    3 => "Directory not found",
                    4 => "File not found",
                    5 => "Temporary error, retries might fix",
                    6 => "Less serious errors",
                    7 => "Fatal error",
                    8 => "Transfer exceeded - limit set by --max-transfer reached",
                    9 => "Operation successful, but no files transferred",
                    10 => "Duration exceeded - limit set by --max-duration reached",
                    _ => "Unknown error"
                };

                throw new Exception($"{msg}: exit code = {proc.ExitCode}");
            }
        }
        finally
        {
            Console.ResetColor();
        }
    }
}
