using VsRemote.Exceptions;
using VsRemote.Interfaces;
using VsRemote.Utils;
using static VsRemote.Utils.VsPath;

namespace VsRemote.Base;

public abstract class VsRemoteFileSystem : IVsRemoteFileSystem
{
    public abstract IVsRemoteINode RootINode { get; }

    public abstract Task CreateDirectory(string directoryName, IVsRemoteINode parentDir, string[] parentPath);

    public abstract Task DeleteFile(IVsRemoteINode fileToDelete, IVsRemoteINode parentDir, string[] parentPath);

    public abstract Task<IEnumerable<IVsRemoteINode>> ListDirectory(IVsRemoteINode dir, string[] path);

    public abstract Task<ReadOnlyMemory<byte>> ReadFile(IVsRemoteINode fileToRead, IVsRemoteINode parentDir, string[] parentPath);
    public virtual async Task<ReadOnlyMemory<byte>> ReadFileOffset(IVsRemoteINode fileToRead, IVsRemoteINode parentDir, string[] parentPath, int offset, int length)
    {
        ReadOnlyMemory<byte> entireFile = await ReadFile(fileToRead, parentDir, parentPath);
        return entireFile.Slice(offset, length);
    }

    public abstract Task RemoveDirectory(IVsRemoteINode dir, string[] path, bool recursive);

    public abstract Task RenameFile(IVsRemoteINode fromFile, string[] fromPath, string toName, string[] toPath);

    public abstract Task<IVsRemoteINode> Stat(string[] path);

    public abstract Task<int> WriteFile(string file2write, IVsRemoteINode parentDir, string[] parentPath, ReadOnlyMemory<byte> content);
    public virtual async Task<int> WriteFileOffset(IVsRemoteINode inode2write, IVsRemoteINode parentDir, string[] parentPath, int offset, ReadOnlyMemory<byte> content)
    {
        ReadOnlyMemory<byte> entireFile = await ReadFile(inode2write, parentDir, parentPath);
        if (offset > content.Length)
            offset = content.Length; // ... what else?!
        int newlen = entireFile.Length - offset + content.Length;
        byte[] newfile = new byte[newlen];
        entireFile[..offset].CopyTo(newfile);
        content.Span.CopyTo(newfile.AsSpan()[offset..]);
        return await WriteFile(inode2write.Name, parentDir, parentPath, newfile);
    }

    public virtual async Task<int> WriteFileAppend(IVsRemoteINode inode2write, IVsRemoteINode parentDir, string[] parentPath, ReadOnlyMemory<byte> content)
    {
        ReadOnlyMemory<byte> entireFile = await ReadFile(inode2write, parentDir, parentPath);
        int newlen = entireFile.Length + content.Length;
        byte[] newfile = new byte[newlen];
        entireFile.CopyTo(newfile);
        content.Span.CopyTo(newfile.AsSpan().Slice(entireFile.Length));
        return await WriteFile(inode2write.Name, parentDir, parentPath, newfile);
    }


    private async Task<(IVsRemoteINode INode, string[] Path)> GetParent(string[] path_a)
    {
        if (path_a.Length == 1)
        {
            return (INode: RootINode, Path: ROOT_PATH);
        }
        else
        {
            var parent_path_a = path_a.SkipLastA();
            return (INode: await Stat(parent_path_a), Path: parent_path_a);
        }
    }
    private async Task<(IVsRemoteINode INode, string[] Path)> GetParentDirectory(string[] path_a)
    {
        var tmp = await GetParent(path_a);
        if (tmp.INode.IsDirectory())
            return tmp;
        throw new NotADirectory();
    }

    #region IVsRemoteFileSystem
    IVsRemoteINode IVsRemoteFileSystem.RootINode => RootINode;

    async Task IVsRemoteFileSystem.CreateDirectory(string path)
    {
        var path_a = Split(path);
        if (path_a.Length == 0)
            throw new InvalidPath();

        var parent = await GetParentDirectory(path_a);
        await CreateDirectory(path_a.Last(), parent.INode, parent.Path);
    }

    async Task IVsRemoteFileSystem.DeleteFile(string path)
    {
        var path_a = Split(path);
        if (path_a.Length == 0)
            throw new InvalidPath();

        var stat_res = await Stat(path_a);
        stat_res.AssertNotDirectory();

        var parent = await GetParentDirectory(path_a);

        await DeleteFile(stat_res, parent.INode, parent.Path);
    }

    async Task<IEnumerable<IVsRemoteINode>> IVsRemoteFileSystem.ListDirectory(string path)
    {
        var path_a = Split(path);
        if (path_a.Length == 0)
        {
            return await ListDirectory(RootINode, ROOT_PATH);
        }
        else
        {
            var dir = await Stat(path_a);
            return await ListDirectory(dir, path_a);
        }
    }

    async Task<ReadOnlyMemory<byte>> IVsRemoteFileSystem.ReadFile(string path)
    {
        var path_a = Split(path);
        if (path_a.Length == 0)
        {
            throw new IsADirectory(); // root IS a directory
        }
        else
        {
            var file = await Stat(path_a);
            file.AssertNotDirectory();
            var containingDir = await GetParentDirectory(path_a);
            return await ReadFile(file, containingDir.INode, containingDir.Path);
        }
    }

    async Task<ReadOnlyMemory<byte>> IVsRemoteFileSystem.ReadFileOffset(string path, int offset, int length)
    {
        var path_a = Split(path);
        if (path_a.Length == 0)
        {
            throw new IsADirectory(); // root IS a directory
        }
        else
        {
            var file = await Stat(path_a);
            file.AssertNotDirectory();
            var containingDir = await GetParentDirectory(path_a);
            return await ReadFileOffset(file, containingDir.INode, containingDir.Path, offset, length);
        }
    }


    async Task IVsRemoteFileSystem.RemoveDirectory(string path, bool recursive)
    {
        var path_a = Split(path);
        if (path_a.Length == 0)
        {
            throw new PermissionDenied(); // you can't delete root folder!
        }
        else
        {
            var dir = await Stat(path_a);
            dir.AssertDirectory();
            if (recursive || !(await ListDirectory(dir, path_a)).Any())
                await RemoveDirectory(dir, path_a, recursive);
            else
                throw new NotEmpty();
        }
    }

    async Task IVsRemoteFileSystem.RenameFile(string fromPath, string toPath, bool overwrite)
    {
        var fromPath_a = Split(fromPath);
        if (fromPath_a.Length == 0)
            throw new InvalidPath();
        var toPath_a = Split(fromPath);
        if (toPath_a.Length == 0)
            throw new InvalidPath();
        var fromInode = await Stat(fromPath_a);
        var fromContainer = await GetParentDirectory(toPath_a);
        var toContainer = await GetParentDirectory(toPath_a);
        bool exists;
        try
        {
            (await Stat(toPath_a)).AssertNotDirectory();
            exists = true;
        } catch (NotFound)
        {
            exists = false;
        }
        if ((exists && overwrite) || !exists)
        {
            await RenameFile(fromInode, fromContainer.Path, toPath_a.Last(), toContainer.Path);
        }
        else
        {
            throw new PermissionDenied();
        }
    }

    async Task<IVsRemoteINode> IVsRemoteFileSystem.Stat(string path)
    {
        var path_a = Split(path);
        if (path_a.Length == 0)
        {
            path_a = ROOT_PATH;
        }
        return await Stat(path_a);
    }

    async Task<int> IVsRemoteFileSystem.WriteFile(string path, ReadOnlyMemory<byte> content, bool overwriteIfExists, bool createIfNotExists)
    {
        var path_a = Split(path);
        if (path_a.Length == 0)
            throw new InvalidPath();

        var parent = await GetParentDirectory(path_a);
        bool exists;
        try
        {
            await Stat(path_a);
            exists = true;
        } catch (NotFound)
        {
            exists = false;
        }
        if ((exists && overwriteIfExists) || (!exists && createIfNotExists))
        {
            return await WriteFile(path_a.Last(), parent.INode, parent.Path, content);
        }
        else
        {
            throw new NotFound();
        }
    }

    async Task<int> IVsRemoteFileSystem.WriteFileOffset(string path, int offset, ReadOnlyMemory<byte> content)
    {
        var path_a = Split(path);
        if (path_a.Length == 0)
            throw new InvalidPath();

        var parent = await GetParentDirectory(path_a);
        IVsRemoteINode inode2write = await ((IVsRemoteFileSystem)this).Stat(path);
        return await WriteFileOffset(inode2write, parent.INode, parent.Path, offset, content);
    }

    async Task<int> IVsRemoteFileSystem.WriteFileAppend(string path, ReadOnlyMemory<byte> content)
    {
        var path_a = Split(path);
        if (path_a.Length == 0)
            throw new InvalidPath();

        var parent = await GetParentDirectory(path_a);
        IVsRemoteINode inode2write = await ((IVsRemoteFileSystem)this).Stat(path);
        return await WriteFileAppend(inode2write, parent.INode, parent.Path, content);
    }
    #endregion
}
