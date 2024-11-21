using VsRemote.Base;
using VsRemote.Enums;
using VsRemote.Exceptions;
using VsRemote.Interfaces;
using VsRemote.Model;
using VsRemote.Utils;

namespace VsRemote.Sample;

public class LocalFolderFilesystem : VsRemoteFileSystem
{
    private readonly string basePath;
    public LocalFolderFilesystem(string base_path)
    {
        basePath = base_path;
    }

    private static readonly IVsRemoteINode _root = new VsRemoteRootINode(
        CTime: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        MTime: DateTimeOffset.UtcNow.ToUnixTimeSeconds()
    );

    private async Task LocalCreateDirectory(string path)
    {
        await Task.Run(() =>
        {
            Directory.CreateDirectory(path);
        });
    }

    private async Task LocalDeleteFile(string path)
    {
        await Task.Run(() =>
        {
            File.Delete(path);
        });
    }

    private async Task<IEnumerable<IVsRemoteINode>> LocalListDirectory(string path)
    {
        IEnumerable<string> dirFiles = Directory.EnumerateFileSystemEntries(path);
        List<IVsRemoteINode> inodes = new(dirFiles.Count());
        foreach (var fname in dirFiles)
        {
            inodes.Add(await LocalStat(fname));
        }
        return inodes;
    }

    private async Task<ReadOnlyMemory<byte>> LocalReadFile(string path)
    {
        return new(await File.ReadAllBytesAsync(path));
    }

    private Task LocalRemoveDirectory(string path, bool recursive)
    {
        Directory.Delete(path, recursive);
        return Task.CompletedTask;
    }

    private Task LocalRenameFile(string fromPath, string toPath)
    {
        File.Move(fromPath, toPath, true);
        return Task.CompletedTask;
    }

    private async Task<IVsRemoteINode> LocalStat(string path)
    {
        if (Directory.Exists(path) || File.Exists(path))
        {
            return await Task.Run(() =>
                new VsRemoteINode(
                    Path.GetFileName(path),
                    Directory.Exists(path) ? VsRemoteFileType.Directory : VsRemoteFileType.File,
                    Readonly: (File.GetAttributes(path) | FileAttributes.ReadOnly) != 0,
                    ((DateTimeOffset)File.GetCreationTimeUtc(path)).ToUnixTimeSeconds(),
                    ((DateTimeOffset)File.GetLastWriteTimeUtc(path)).ToUnixTimeSeconds(),
                    ((DateTimeOffset)File.GetLastAccessTimeUtc(path)).ToUnixTimeSeconds()
            ));
        }
        else
        {
            throw new NotFound();
        }
    }

    private async Task<int> LocalWriteFile(string path, ReadOnlyMemory<byte> content)
    {
        var stream = File.OpenWrite(path);
        stream.Seek(0, SeekOrigin.Begin);
        await stream.WriteAsync(content);
        stream.Close();
        return content.Length;
    }

    private string LocalPath(string path)
        => Path.Join(basePath, path.Replace(VsPath.PATH_SEPARATOR, Path.DirectorySeparatorChar));

    #region VsRemoteFileSystem

    public override IVsRemoteINode RootINode => _root;

    public override Task CreateDirectory(string directoryName, IVsRemoteINode parentDir, string[] parentPath)
    {
        return LocalCreateDirectory(LocalPath(VsPath.Join(parentPath, directoryName)));
    }

    public override Task DeleteFile(IVsRemoteINode fileToDelete, IVsRemoteINode parentDir, string[] parentPath)
    {
        return LocalDeleteFile(LocalPath(VsPath.Join(parentPath, fileToDelete.Name)));
    }

    public override Task<IEnumerable<IVsRemoteINode>> ListDirectory(IVsRemoteINode dir, string[] path)
    {
        return LocalListDirectory(LocalPath(VsPath.Join(path)));
    }

    public override Task<ReadOnlyMemory<byte>> ReadFile(IVsRemoteINode fileToRead, IVsRemoteINode parentDir, string[] parentPath)
    {
        return LocalReadFile(LocalPath(VsPath.Join(parentPath, fileToRead.Name)));
    }

    public override Task RemoveDirectory(IVsRemoteINode dir, string[] path, bool recursive)
    {
        return LocalRemoveDirectory(LocalPath(VsPath.Join(path, dir.Name)), recursive);
    }

    public override Task RenameFile(IVsRemoteINode fromFile, string[] fromPath, string toName, string[] toPath)
    {
        return LocalRenameFile(
            LocalPath(VsPath.Join(fromPath, fromFile.Name)),
            LocalPath(VsPath.Join(toPath, toName))
            );
    }

    public override Task<IVsRemoteINode> Stat(string[] path)
    {
        return LocalStat(LocalPath(VsPath.Join(path)));
    }

    public override Task<int> WriteFile(string file2write, IVsRemoteINode parentDir, string[] parentPath, ReadOnlyMemory<byte> content)
    {
        return LocalWriteFile(LocalPath(VsPath.Join(parentPath, file2write)), content);
    }

    #endregion
}
