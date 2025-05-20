using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace MyBackupUtil.ConfigOptions;

internal class Config
{
    public string RcloneRemote { get; set; } = string.Empty;

    public string? RcloneBackup { get; set; }

    public string? RcloneFlags { get; set; }

    public bool IncludeConfigFileInBackup { get; set; }

    public List<string> Files { get; set; } = [];

    public List<DirectoryBackup> Directories { get; set; } = [];

    public void Save(string? filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
            filename = DefaultConfigFile;
        new FileInfo(filename).WriteAllText(JsonSerializer.Serialize(this, SourceGenerationContext.Default.Config));
    }



    public static string DefaultConfigFile => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mybackuputil.json");


    public static Config Load(string? filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
            filename = DefaultConfigFile;

        Console.WriteLine("Using config file: ");
        Console.WriteLine(filename);

        try
        {
            return JsonSerializer.Deserialize<Config>(new FileInfo(filename).ReadAllText(), SourceGenerationContext.Default.Config)!;
        }
        catch { return new(); }
    }
}
