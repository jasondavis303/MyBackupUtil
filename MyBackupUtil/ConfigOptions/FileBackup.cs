namespace MyBackupUtil.ConfigOptions;

internal class FileBackup
{
    public string Filename { get; set; } = string.Empty;

    public bool ErrorIfMissing { get; set; }
}
