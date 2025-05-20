using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System.IO;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class FileSystemExtensions
{
    #region DirectoryInfo

    public static DirectoryInfo GetDirectoryInfo(this Environment.SpecialFolder self, Environment.SpecialFolderOption option = Environment.SpecialFolderOption.None) =>
        new(Environment.GetFolderPath(self, option));

    /// <summary>
    /// Returns a new <see cref="DirectoryInfo"/> without creating the directories
    /// </summary>
    public static DirectoryInfo SubDirectory(this DirectoryInfo self, params string[] parts)
    {
        var partsLst = parts.ToList();
        partsLst.Insert(0, self.FullName);
        return new DirectoryInfo(Path.Combine([.. partsLst]));
    }

    /// <summary>
    /// Returns a new <see cref="FileInfo"/> without creating the file
    /// </summary>
    public static FileInfo SubFile(this DirectoryInfo self, params string[] parts)
    {
        var partsLst = parts.ToList();
        partsLst.Insert(0, self.FullName);
        return new FileInfo(Path.Combine([.. partsLst]));
    }

    /// <summary>
    /// Deletes all empty folders in the sub-tree the specified <paramref name="self"/>
    /// </summary>
    /// <param name="self"></param>
    public static void DeleteEmptyChildren(this DirectoryInfo self)
    {
        try
        {
            self.Refresh();

            if (!self.Exists)
                return;

            foreach (var subDir in self.GetDirectories())
            {
                subDir.DeleteEmptyChildren();
                subDir.TryDelete(false);
            }
        }
        catch { }
    }

    /// <summary>
    /// Tries to delete a directory without throwing an exception on failure
    /// </summary>
    public static bool TryDelete(this DirectoryInfo self, bool recursive = false)
    {
        try
        {
            self.Delete(recursive);
            self.Refresh();
            return true;
        }
        catch { }

        return false;
    }


    #endregion




    #region Writing Files


    public static void WriteAllText(this FileInfo self, string text)
    {
        self.Directory!.Create();
        File.WriteAllText(self.FullName, text, Encoding.UTF8);
        self.Refresh();
    }

    public static void WriteAllText(this FileInfo self, StringBuilder sb) =>
        self.WriteAllText(sb.ToString());

    public static void AppendAllText(this FileInfo self, string text)
    {
        self.Directory!.Create();
        File.AppendAllText(self.FullName, text, Encoding.UTF8);
        self.Refresh();
    }

    public static void AppendAllText(this FileInfo self, StringBuilder sb) =>
        self.AppendAllText(sb.ToString());

    public static void WriteAllLines(this FileInfo self, IEnumerable<string> contents)
    {
        self.Directory!.Create();
        File.WriteAllLines(self.FullName, contents, Encoding.UTF8);
        self.Refresh();
    }

    public static void AppendAllLines(this FileInfo self, IEnumerable<string> lines)
    {
        self.Directory!.Create();
        File.WriteAllLines(self.FullName, lines, Encoding.UTF8);
        self.Refresh();
    }

    public static void WriteAllBytes(this FileInfo self, byte[] bytes)
    {
        self.Directory!.Create();
        File.WriteAllBytes(self.FullName, bytes);
        self.Refresh();
    }


    //static readonly JsonSerializerOptions _defaultJsonWriteOptions = new() { WriteIndented = true };

    //public static void WriteJson(this FileInfo self, object data) =>
    //    self.WriteAllText(JsonSerializer.Serialize(data, _defaultJsonWriteOptions));

    //public static void WriteJson(this FileInfo self, object data, JsonSerializerOptions options) =>
    //    self.WriteAllText(JsonSerializer.Serialize(data, options));


    //public static void WriteXml(this FileInfo self, object data)
    //{
    //    self.Directory!.Create();
    //    using var fs = File.CreateText(self.FullName);
    //    var xs = new Xml.Serialization.XmlSerializer(data.GetType());
    //    xs.Serialize(fs, data);
    //}

    #endregion



    #region Reading Files


    public static string ReadAllText(this FileInfo self) => File.ReadAllText(self.FullName);

    public static byte[] ReadAllBytes(this FileInfo self) => File.ReadAllBytes(self.FullName);

    public static string[] ReadAllLines(this FileInfo self) => File.ReadAllLines(self.FullName);



    //static readonly JsonSerializerOptions _defaultJsonReadOptions = new(JsonSerializerDefaults.Web);

    //public static T ReadJson<T>(this FileInfo self) => self.ReadJson<T>(_defaultJsonReadOptions);

    //public static bool TryReadJson<T>(this FileInfo self, out T? data)
    //{
    //    try
    //    {
    //        data = self.ReadJson<T>();
    //        return true;
    //    }
    //    catch 
    //    {
    //        data = default;
    //        return false;
    //    }
    //}

    //public static T ReadJson<T>(this FileInfo self, JsonSerializerOptions options) =>
    //    JsonSerializer.Deserialize<T>(self.ReadAllText(), options)!;

    //public static bool TryReadJson<T>(this FileInfo self, JsonSerializerOptions options, out T? data)
    //{
    //    try
    //    {
    //        data = self.ReadJson<T>(options);
    //        return true;
    //    }
    //    catch
    //    {
    //        data = default; 
    //        return false;
    //    }
    //}

    //public static T ReadXml<T>(this FileInfo self)
    //{
    //    using var fs = self.OpenText();
    //    var xs = new Xml.Serialization.XmlSerializer(typeof(T));
    //    return (T)xs.Deserialize(fs)!;
    //}

    //public static bool TryReadXml<T>(this FileInfo self, out T? data)
    //{
    //    try
    //    {
    //        data = self.ReadXml<T>();
    //        return true;
    //    }
    //    catch
    //    {
    //        data = default;
    //        return false;
    //    }
    //}

    #endregion






    #region Operations

    [Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public static void AddExecutePermission(this FileInfo self)
    {
        if (Environment.OSVersion.Platform != PlatformID.Win32NT)
        {
            var ufm = File.GetUnixFileMode(self.FullName);
            ufm |= UnixFileMode.UserExecute | UnixFileMode.GroupExecute | UnixFileMode.OtherExecute;
            File.SetUnixFileMode(self.FullName, ufm);
        }
    }

    /// <summary>
    /// Returns a new <see cref="FileInfo"/> with the new <paramref name="newExtension"/>. This does NOT modify files on disk
    /// </summary>
    public static FileInfo ChangeExtension(this FileInfo self, string newExtension) => new(Path.ChangeExtension(self.FullName, newExtension));

    /// <summary>
    /// Returns a new <see cref="FileInfo"/> with the new <paramref name="newName"/>. This does NOT modify files on disk
    /// </summary>
    public static FileInfo ChangeFilename(this FileInfo self, string newName) => new(Path.Combine(self.DirectoryName!, newName));

    public static string GetFilenameWithoutExtension(this FileInfo fileInfo) => Path.GetFileNameWithoutExtension(fileInfo.Name);



    static FileInfo GetTempFileForCopy(this FileInfo fileInfo)
    {
        var tmpFile = new FileInfo(fileInfo.FullName + ".tmp");
        if (!tmpFile.Exists)
            return tmpFile;

        int idx = 0;
        while (true)
        {
            tmpFile = new FileInfo(fileInfo.FullName + $".tmp{idx++}");
            if (!tmpFile.Exists)
                return tmpFile;
        }
    }


    public static void CopyTo(this FileInfo self, FileInfo dst, bool overwrite = false, bool useTmpFile = false)
    {
        dst.Directory!.Create();

        var copyToDst = useTmpFile ? GetTempFileForCopy(dst) : dst;
        File.Copy(self.FullName, copyToDst.FullName, overwrite);

        if (useTmpFile)
            File.Move(copyToDst.FullName, dst.FullName, overwrite);

        dst.Refresh();
    }


    public static async Task CopyToAsync(this FileInfo self, FileInfo dst, bool overwrite = false, IProgress<int>? progress = null, bool useTmpFile = false, CancellationToken cancellationToken = default)
    {
        self.Refresh();
        using var srcStream = self.OpenRead();

        dst.Directory!.Create();
        var copyToDst = useTmpFile ? GetTempFileForCopy(dst) : dst;

        var options = new FileStreamOptions
        {
            Mode = overwrite ? FileMode.CreateNew : FileMode.Create,
            Access = FileAccess.Write,
            Options = FileOptions.Asynchronous | FileOptions.SequentialScan,
            BufferSize = 4096,
            PreallocationSize = self.Length,
            Share = FileShare.None
        };

        using (var dstStream = new FileStream(copyToDst.FullName, options))
        {
            var buffer = new byte[4096];
            double totalRead = 0;
            double totalLength = self.Length;
            int lastDL = -1;

            while (true)
            {
                int read = await srcStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
                if (read < 1)
                    break;

                await dstStream.WriteAsync(buffer.AsMemory(0, read), cancellationToken).ConfigureAwait(false);

                if (progress != null)
                {
                    totalRead += read;
                    var newDL = Math.Max(0, Math.Min(100, Convert.ToInt32(totalRead / totalLength * 100d)));
                    if (newDL > lastDL)
                    {
                        lastDL = newDL;
                        progress.Report(newDL);
                    }
                }
            }
        }

        if (useTmpFile)
            File.Move(copyToDst.FullName, dst.FullName, overwrite);

        dst.Refresh();
    }


    public static void MoveTo(this FileInfo self, FileInfo dst, bool overwrite = false, bool useTmpFile = false)
    {
        dst.Directory!.Create();

        var copyToDst = useTmpFile ? GetTempFileForCopy(dst) : dst;
        File.Move(self.FullName, copyToDst.FullName, overwrite);

        if (useTmpFile)
            File.Move(copyToDst.FullName, dst.FullName, overwrite);

        dst.Refresh();
    }


    public static async Task MoveToAsync(this FileInfo self, FileInfo dst, bool overwrite = false, IProgress<int>? progress = null, bool useTmpFile = false, CancellationToken cancellationToken = default)
    {
        await CopyToAsync(self, dst, overwrite, progress, useTmpFile, cancellationToken).ConfigureAwait(false);
        self.Delete();
    }


    /// <summary>
    /// Tries to delete a file without throwing an exception on failure
    /// </summary>
    public static bool TryDelete(this FileInfo self)
    {
        try
        {
            self.Refresh();
            self.Delete();
            return true;
        }
        catch { }

        return false;
    }


    public static bool TryLockFile(this FileInfo self, out IDisposable? lockedFile)
    {
        try
        {
            self.Directory!.Create();
            lockedFile = self.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            return true;
        }
        catch { }

        lockedFile = null;
        return false;
    }



    public static bool IsFileLocked(this FileInfo self)
    {
        self.Refresh();
        if (!self.Exists)
            return false;

        try
        {
            using var fs = self.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            return false;
        }
        catch
        {
            return true;
        }
    }


    public static void Touch(this FileInfo self)
    {
        self.Refresh();
        if (!self.Exists)
        {
            self.Directory!.Create();
            self.Create().Dispose();
        }
        self.LastAccessTime = DateTime.Now;
        self.LastWriteTime = DateTime.Now;
        self.CreationTime = DateTime.Now;
    }

    #endregion
}