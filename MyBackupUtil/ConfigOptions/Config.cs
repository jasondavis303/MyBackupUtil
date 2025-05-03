using System.Collections.Generic;

namespace MyBackupUtil.ConfigOptions;

internal class Config
{
    public string RcloneRemote { get; set; } = string.Empty;

    public string? RcloneBackup { get; set; }

    public string? RcloneFlags { get; set; }

    public List<FileBackup> Files { get; set; } = [];

    public List<DirectoryBackup> Directories { get; set; } = [];
}
