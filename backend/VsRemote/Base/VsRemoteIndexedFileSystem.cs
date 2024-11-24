﻿using VsRemote.Exceptions;
using VsRemote.Interfaces;
using VsRemote.Model;
using VsRemote.Utils;
using static VsRemote.Utils.VsPath;

namespace VsRemote.Base;

public abstract class VsRemoteFileSystem<T> : IVsRemoteFileSystem where T: IEquatable<T>
{
    public abstract IVsRemoteINode<T> RootINode { get; }

    public abstract Task CreateDirectory(string directoryName, IVsRemoteINode<T> parentDir);

    public abstract Task DeleteFile(IVsRemoteINode<T> fileToDelete);

    public abstract Task<IEnumerable<IVsRemoteINode<T>>> ListDirectory(IVsRemoteINode<T> dir);

    public virtual async Task<bool> DirectoryIsEmpty(IVsRemoteINode<T> dir)
        => !(await ListDirectory(dir)).Any();

    public abstract Task<ReadOnlyMemory<byte>> ReadFile(IVsRemoteINode<T> fileToRead);
    public virtual async Task<ReadOnlyMemory<byte>> ReadFileOffset(IVsRemoteINode<T> fileToRead, int offset, int length)
    {
        ReadOnlyMemory<byte> entireFile = await ReadFile(fileToRead);
        return entireFile.Slice(offset, Math.Min(length, entireFile.Length));
    }

    public abstract Task RemoveDirectory(IVsRemoteINode<T> dir, bool recursive);

    public abstract Task RenameFile(IVsRemoteINode<T> fromFile, string toName, IVsRemoteINode<T> toPath);
    public abstract Task OverwriteFile(IVsRemoteINode<T> fromFile, IVsRemoteINode<T> toFile);

    public abstract Task<int> CreateFile(string file2write, IVsRemoteINode<T> parentDir, ReadOnlyMemory<byte> content);
    public abstract Task<int> RewriteFile(IVsRemoteINode<T> file2rewrite, ReadOnlyMemory<byte> content);
    public virtual async Task<int> WriteFileOffset(IVsRemoteINode<T> inode2write, int offset, ReadOnlyMemory<byte> content)
    {
        ReadOnlyMemory<byte> entireFile = await ReadFile(inode2write);
        if (offset > content.Length)
            offset = content.Length; // ... what else?!
        int newlen = offset + content.Length;
        byte[] newfile = new byte[newlen];
        entireFile[..offset].CopyTo(newfile);
        content.Span.CopyTo(newfile.AsSpan()[offset..]);
        return await RewriteFile(inode2write, newfile);
    }

    public virtual async Task<int> WriteFileAppend(IVsRemoteINode<T> inode2write, ReadOnlyMemory<byte> content)
    {
        ReadOnlyMemory<byte> entireFile = await ReadFile(inode2write);
        int newlen = entireFile.Length + content.Length;
        byte[] newfile = new byte[newlen];
        entireFile.CopyTo(newfile);
        content.Span.CopyTo(newfile.AsSpan().Slice(entireFile.Length));
        return await RewriteFile(inode2write, newfile);
    }

    public abstract Task<IVsRemoteINode<T>> FindByName(string file, IVsRemoteINode<T> parentDir);

    private Task<IVsRemoteINode<T>> GetLastChild(string[] path_a)
        => GetLastChild(path_a, RootINode);

    private async Task<IVsRemoteINode<T>> GetLastChild(string[] path_a, IVsRemoteINode<T> tmpFile)
    {
        foreach (var path_el in path_a)
        {
            tmpFile.AssertDirectory();
            var tmpInode = await FindByName(path_el, tmpFile);
            tmpFile = tmpInode;
        }
        return tmpFile;
    }

    private Task<IVsRemoteINode<T>> GetParentDirectory(string[] path_a)
        => GetParentDirectory(path_a, RootINode);

    private async Task<IVsRemoteINode<T>> GetParentDirectory(string[] path_a, IVsRemoteINode<T> tmpDir)
    {
        foreach (var path_el in path_a.SkipLastA())
        {
            var tmpInode = await FindByName(path_el, tmpDir);
            tmpDir = tmpInode.AssertDirectory();
        }
        return tmpDir;
    }

    #region IVsRemoteFileSystem
    IVsRemoteINode IVsRemoteFileSystem.RootINode => RootINode;

    async Task IVsRemoteFileSystem.CreateDirectory(string path)
    {
        var path_a = Split(path);
        if (path_a.Length == 0)
            throw new InvalidPath();

        var parentINode = await GetParentDirectory(path_a);
        await CreateDirectory(path_a.Last(), parentINode);
    }

    async Task IVsRemoteFileSystem.DeleteFile(string path)
    {
        var path_a = Split(path);
        if (path_a.Length == 0)
            throw new InvalidPath();

        var fileINode = await GetLastChild(path_a);
        fileINode.AssertNotDirectory();

        await DeleteFile(fileINode);
    }

    async Task<IEnumerable<IVsRemoteINode>> IVsRemoteFileSystem.ListDirectory(string path)
    {
        var path_a = Split(path);
        if (path_a.Length == 0)
        {
            return await ListDirectory(RootINode);
        }
        else
        {
            var dir = await GetLastChild(path_a);
            dir.AssertDirectory();
            return await ListDirectory(dir);
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
            var file = await GetLastChild(path_a);
            file.AssertNotDirectory();
            return await ReadFile(file);
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
            var file = await GetLastChild(path_a);
            file.AssertNotDirectory();
            return await ReadFileOffset(file, offset, length);
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
            var dir = await GetLastChild(path_a);
            dir.AssertDirectory();
            if (recursive || await DirectoryIsEmpty(dir))
                await RemoveDirectory(dir, recursive);
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
        var minCommonPath = new List<string>(Math.Max(fromPath_a.Length, toPath_a.Length));
        for (int i = 0; fromPath_a[i] == toPath_a[i]; i++)
        {
            minCommonPath.Add(fromPath_a[i]);
        }
        var commonPathINode = await GetLastChild(minCommonPath.ToArray()); // array empty => root inode
        var fromInode = await GetLastChild(fromPath_a, commonPathINode);
        var toContainer = await GetParentDirectory(toPath_a, commonPathINode);

        try
        {
            var toINode = await GetLastChild(toPath_a, toContainer);
            toINode.AssertNotDirectory();
            if (overwrite)
            {
                await OverwriteFile(fromInode, toINode);
            }
            else
            {
                throw new PermissionDenied();
            }
        } catch (NotFound)
        {
            await RenameFile(fromInode, toPath_a.Last(), toContainer);
        }
    }

    async Task<IVsRemoteINode> IVsRemoteFileSystem.Stat(string path)
    {
        var path_a = Split(path);
        return await GetLastChild(path_a);
    }

    async Task IVsRemoteFileSystem.CreateFile(string path)
    {
        var path_a = Split(path);
        if (path_a.Length == 0)
        {
            throw new IsADirectory(); // root IS a directory
        }
        else
        {
            var parentDir = await GetParentDirectory(path_a);
            try
            {
                await GetLastChild(path_a, parentDir);
            }
            catch (NotFound)
            {
                await CreateFile(path_a.Last(), parentDir, Array.Empty<byte>());
            }
        }
    }
    async Task<int> IVsRemoteFileSystem.WriteFile(string path, ReadOnlyMemory<byte> content, bool overwriteIfExists, bool createIfNotExists)
    {
        var path_a = Split(path);
        if (path_a.Length == 0)
        {
            throw new IsADirectory(); // root IS a directory
        }
        else
        {
            var parentDir = await GetParentDirectory(path_a);
            try
            {
                var file = await GetLastChild(path_a, parentDir);
                if (overwriteIfExists)
                {
                    file.AssertNotDirectory();
                    return await RewriteFile(file, content);
                }
                else
                {
                    throw new PermissionDenied();
                }
            }
            catch (NotFound)
            {
                if (createIfNotExists)
                {
                    return await CreateFile(path_a.Last(), parentDir, content);
                }
                else
                {
                    throw new PermissionDenied();
                }
            }
        }
    }

    async Task<int> IVsRemoteFileSystem.WriteFileOffset(string path, int offset, ReadOnlyMemory<byte> content)
    {
        var path_a = Split(path);
        if (path_a.Length == 0)
        {
            throw new IsADirectory(); // root IS a directory
        }
        else
        {
            var parentDir = await GetParentDirectory(path_a);
            try
            {
                var file = await GetLastChild(path_a, parentDir);
                file.AssertNotDirectory();
                return await WriteFileOffset(file, offset, content);
            }
            catch (NotFound)
            {
                throw;
            }
        }
    }

    async Task<int> IVsRemoteFileSystem.WriteFileAppend(string path, ReadOnlyMemory<byte> content)
    {
        var path_a = Split(path);
        if (path_a.Length == 0)
        {
            throw new IsADirectory(); // root IS a directory
        }
        else
        {
            var parentDir = await GetParentDirectory(path_a);
            try
            {
                var file = await GetLastChild(path_a, parentDir);
                file.AssertNotDirectory();
                return await WriteFileAppend(file, content);
            }
            catch (NotFound)
            {
                throw;
            }
        }
    }
    #endregion
}
