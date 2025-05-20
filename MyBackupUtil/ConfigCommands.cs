using ConsoleAppFramework;
using MyBackupUtil.ConfigOptions;
using System;
using System.IO;

namespace MyBackupUtil;

/// <summary>
/// Configure backups
/// </summary>
[RegisterCommands]
internal class ConfigCommands
{


    /// <summary>
    /// Configure rclone for backup
    /// </summary>
    /// <param name="remote">-r, The remote destination for backups</param>
    /// <param name="backup">-b, The remote destination for backups of backups</param>
    /// <param name="configFile">-c, Config file</param>
    /// <param name="flags">-f, Flags to send to rclone (prepend each flag with x)</param>
    public void ConfigureRclone(string remote, string? backup = null, string? configFile = null, string[]? flags = null)
    {
        string? rcFlags = flags == null ? null : string.Join(' ', flags);

        if (string.IsNullOrWhiteSpace(backup))
            backup = remote + ".bak";


        Config config = Config.Load(configFile);

        Console.WriteLine("Configuring rclone:");
        Console.WriteLine($"\tRemote: {remote}");
        Console.WriteLine($"\tBackup: {backup}");
        Console.WriteLine($"\tFlags: {rcFlags}");

        config.RcloneRemote = remote;
        config.RcloneBackup = backup;
        if (flags != null)
            config.RcloneFlags = rcFlags;
        config.Save(configFile);
    }

    /// <summary>
    /// Add a directory for backup
    /// </summary>
    /// <param name="directory">-d, Directory to backup</param>
    /// <param name="configFile">-c, Config file</param>
    /// <param name="include">-i, Rclone include</param>
    /// <param name="exclude">-e, Rclone exclude</param>
    public void AddDirectory([Argument] string directory, string? configFile = null, string[]? include = null, string[]? exclude = null)
    {
        Config config = Config.Load(configFile);

        Console.WriteLine("Adding directory:");
        Console.WriteLine($"\t{directory}");

        if (include != null)
            Console.WriteLine($"\tInclude: {string.Join(' ', include)}");

        if (exclude != null)
            Console.WriteLine($"\tExclude: {string.Join(' ', exclude)}");

        config.Directories.RemoveAll(d => d.Directory == directory);

        DirectoryBackup directoryBackup = new DirectoryBackup { Directory = directory };
        if (include != null)
            directoryBackup.RcloneIncludes.AddRange(include);
        if (exclude != null)
            directoryBackup.RcloneIncludes.AddRange(exclude);

        config.Save(configFile);
    }

    /// <summary>
    /// Remove a directory from backup
    /// </summary>
    /// <param name="directory">-d, Directory to remove</param>
    /// <param name="configFile">-c, Config file</param>
    public void RemoveDirectory([Argument] string directory, string? configFile = null)
    {
        Config config = Config.Load(configFile);

        Console.WriteLine("Removing directory:");
        Console.WriteLine($"\t{directory}");

        config.Directories.RemoveAll(d => d.Directory == directory);
        config.Save(configFile);
    }

    /// <summary>
    /// Add a file for backup
    /// </summary>
    /// <param name="file">-f, File to backup</param>
    /// <param name="configFile">-c, Config file</param>
    public void AddFile([Argument] string file, string? configFile = null)
    {
        Config config = Config.Load(configFile);

        Console.WriteLine("Adding file:");
        Console.WriteLine($"\t{file}");

        if (!config.Files.Contains(file))
        {
            config.Files.Add(file);
            config.Save(configFile);
        }
    }

    /// <summary>
    /// Remove a file from backup
    /// </summary>
    /// <param name="file">-f, File to remove</param>
    /// <param name="configFile">-c, Config file</param>
    public void RemoveFile([Argument] string file, string? configFile = null)
    {
        Config config = Config.Load(configFile);

        Console.WriteLine("Removing file:");
        Console.WriteLine($"\t{file}");

        if (config.Files.Contains(file))
        {
            config.Files.Remove(file);
            config.Save(configFile);
        }
    }


    /// <summary>
    /// Include the config file in backups
    /// </summary>
    /// <param name="configFile">-c, Config file</param>
    public void IncludeConfigInBackup(string? configFile = null)
    {
        Config config = Config.Load(configFile);

        Console.WriteLine("Including config");
        config.IncludeConfigFileInBackup = true;
        config.Save(configFile);
    }

    /// <summary>
    /// Exclude the config file from backups
    /// </summary>
    /// <param name="configFile">-c, Config file</param>
    public void ExcludeConfigFromBackup(string? configFile = null)
    {
        Config config = Config.Load(configFile);

        Console.WriteLine("Excluding config");
        config.IncludeConfigFileInBackup = false;
        config.Save(configFile);
    }




    /// <summary>
    /// Prints the location and contents of the config file 
    /// </summary>
    /// <param name="configFile">-c, Config file</param>
    public void DumpConfig(string? configFile = null)
    {
        if (string.IsNullOrWhiteSpace(configFile))
            configFile = Config.DefaultConfigFile;
        Console.WriteLine(configFile);
        if (File.Exists(configFile))
            Console.WriteLine(File.ReadAllText(configFile));
    }
}
