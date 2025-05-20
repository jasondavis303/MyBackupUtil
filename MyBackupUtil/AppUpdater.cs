using MyBackupUtil.CLOptions;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MyBackupUtil;

internal class AppUpdater
{
    const string URL_ROOT = "https://s3.dustypig.tv/bin/mybackuputil/MyBackupUtil";
    const string VERSION_URL = URL_ROOT + ".version";
    const string WINDOWS_URL = URL_ROOT + ".exe";
    const string LINUX_URL = URL_ROOT;

    public static async Task Update(UpdateOptions options)
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

            string url = Environment.OSVersion.Platform switch
            {
                PlatformID.Win32NT => WINDOWS_URL,
                _ => LINUX_URL,
            };

            await SimpleDownloader.DownloadFileAsync(url, Environment.GetCommandLineArgs()[0]);
            Console.WriteLine("Success");
        }
        else
        {
            Console.WriteLine($"Current version {currentVersion} is up to date");
        }
    }
}