using ConsoleAppFramework;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace MyBackupUtil;

[RegisterCommands]
internal class AppUpdater
{
    public const string MAGIC_KEY = "73F426D6D5D64F39B3379EC6F867DB59";

    const string URL_ROOT = "https://s3.dustypig.tv/bin/mybackuputil/mybackuputil";
    const string VERSION_URL = URL_ROOT + ".version";
    const string WINDOWS_URL = URL_ROOT + ".exe";
    const string LINUX_URL = URL_ROOT;

    /// <summary>
    /// Try to update the program
    /// </summary>
    public async Task Update()
    {
        Console.WriteLine("Checking for updates");

        Version currentVersion = new();
        try { currentVersion = Assembly.GetExecutingAssembly().GetName().Version!; }
        catch { }

        Version newVersion;
        try
        {
            string s = await SimpleDownloader.DownloadStringAsync(VERSION_URL);
            newVersion = new(s);
        }
        catch
        {
            throw new Exception("Cannot get version information from the server");
        }

        if (newVersion > currentVersion)
        {
            Console.WriteLine($"Updating to: {newVersion}");

            bool windows = Environment.OSVersion.Platform == PlatformID.Win32NT;

            string url = windows ? WINDOWS_URL : LINUX_URL;

            string dst = Path.Combine(Path.GetTempPath(), MAGIC_KEY);
            if (windows)
                dst += ".exe";

            await SimpleDownloader.DownloadFileAsync(url, dst);

            new FileInfo(dst).AddExecutePermission();

            string[] args =
            [
                MAGIC_KEY,
                Environment.ProcessId.ToString(),
                Environment.GetCommandLineArgs()[0]
            ];
            Process.Start(dst, args);

            Console.WriteLine("Success");
        }
        else
        {
            Console.WriteLine($"Current version {currentVersion} is up to date");
        }
    }


    internal static bool WaitAndUpdate(string[]? args)
    {
        if(args is null)
            return false;

        if (args.Length != 3)
            return false;

        if (args[0] != MAGIC_KEY)
            return false;

        if (!int.TryParse(args[1], out int pid))
            return false;

        try { Process.GetProcessById(pid).WaitForExit(); }
        catch { }

        File.Copy(Environment.GetCommandLineArgs()[0], args[2], true);
        new FileInfo(args[2]).AddExecutePermission();

        return true;
    }
}