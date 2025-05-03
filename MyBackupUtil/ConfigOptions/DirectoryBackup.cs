using System.Collections.Generic;

namespace MyBackupUtil.ConfigOptions;

internal class DirectoryBackup
{
    public string Directory { get; set; } = string.Empty;

    public bool ErrorIfMissing { get; set; }

    /// <summary>
    /// Only use this OR <see cref="RcloneExcludes"/>.
    /// Do not prefix with the --include, it's done by the backup util.
    /// The backup util will NOT check for quoted arguments, make sure you do it here!
    /// </summary>
    public List<string> RcloneIncludes { get; set; } = [];

    /// <summary>
    /// Only use this OR <see cref="RcloneIncludes"/>
    /// Do not prefix with the --include, it's done by the backup util.
    /// The backup util will NOT check for quoted arguments, make sure you do it here!
    /// </summary>
    public List<string> RcloneExcludes { get; set; } = [];
}
