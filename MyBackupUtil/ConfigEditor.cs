using MyBackupUtil.CLOptions;
using MyBackupUtil.ConfigOptions;
using System;
using System.IO;
using System.Linq;

namespace MyBackupUtil;

internal static class ConfigEditor
{
    static Config Load(FileInfo config)
    {
        try
        {
            return config.ReadJson<Config>();
        }
        catch
        {
            return new();
        }
    }

    public static void Print(PrintOptions options)
    {
        Console.WriteLine();
        Console.WriteLine(options.Config.ReadAllText());
        Console.WriteLine();
    }


    public static void AddDirectory(AddDirectoryOptions options)
    {
        Config config = Load(options.Config);

        config.Directories.RemoveAll(_ => _.Directory == options.Path);
        config.Directories.Add(new()
        {
            Directory = options.Path,
            ErrorIfMissing = options.ErrorIfMissing,
            RcloneIncludes = options.Includes.ToList(),
            RcloneExcludes = options.Excludes.ToList()
        });
        options.Config.WriteJson(config);
        Console.WriteLine("Directory added");
    }


    public static void RemoveDirectory(RemoveDirectoryOptions options)
    {
        Config config = Load(options.Config);

        if (config.Directories.RemoveAll(_ => _.Directory == options.Path) > 0)
            options.Config.WriteJson(config);
        Console.WriteLine("Directory removed");
    }


    public static void AddFile(AddFileOptions options)
    {
        Config config = Load(options.Config);

        config.Files.RemoveAll(_ => _.Filename == options.Path);
        config.Files.Add(new()
        {
            Filename = options.Path,
            ErrorIfMissing = options.ErrorIfMissing
        });
        options.Config.WriteJson(config);
        Console.WriteLine("File added");
    }


    public static void RemoveFile(RemoveFileOptions options)
    {
        Config config = Load(options.Config);

        if (config.Files.RemoveAll(_ => _.Filename == options.Path) > 0)
            options.Config.WriteJson(config);
        Console.WriteLine("File removed");
    }


    public static void SetRclone(SetRcloneOptions options)
    {
        Config config = Load(options.Config);

        config.RcloneRemote = options.Remote;
        config.RcloneBackup = options.Backup;

        string? flags = (options.Flags + string.Empty).Trim('"');
        if (string.IsNullOrWhiteSpace(flags))
            flags = null;

        config.RcloneFlags = flags;
        options.Config.WriteJson(config);
        Console.WriteLine("Rclone config saved");
    }
}
