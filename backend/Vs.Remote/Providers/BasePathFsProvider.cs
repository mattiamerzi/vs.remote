using Vs.Remote.Enums;
using Vs.Remote.Exceptions;
using Vs.Remote.Interfaces;
using Vs.Remote.Model;
using Vs.Remote.Utils;

namespace Vs.Remote.Providers;

public class BasePathFsProvider : IVsRemoteFileSystemProvider
{
    private readonly Dictionary<string, IVsRemoteFileSystem> remoteFilesystems;
    private readonly IVsRemoteFileSystem virtualRootFs;

    public BasePathFsProvider(Dictionary<string, IVsRemoteFileSystem> rootFss)
    {
        foreach (var fs in rootFss)
        {
            if (VsPath.Split(fs.Key).Length != 1)
                throw new InvalidPath("Subdir filesystem mount point is not supported, only first level folders");
            if (fs.Value == null)
                throw new NullReferenceException("'null' filesystem? wtf?!");
        }
        remoteFilesystems = rootFss;
        virtualRootFs = new ReadOnlyVirtualRoot(remoteFilesystems);
    }

    public (string RelativePath, IVsRemoteFileSystem RemoteFs) FromPath(string path, string? auth_token)
    {
        if (string.IsNullOrEmpty(path) || VsPath.IsRoot(path))
        {
            return (RelativePath: path, RemoteFs: virtualRootFs);
        }
        else
        {
            string[] components = VsPath.Split(path);
            if (remoteFilesystems.TryGetValue(components[0], out IVsRemoteFileSystem? vsRemote))
            {
                return (RelativePath: VsPath.RemoveFirstDir(components), RemoteFs: vsRemote);
            }
            else
            {
                throw new NotFound();
            }
        }
    }

    private class ReadOnlyVirtualRoot : IVsRemoteFileSystem
    {
        private readonly Dictionary<string, IVsRemoteFileSystem> remoteFilesystems;

        public IVsRemoteINode RootINode
        {
            get
            {
                long mtime = remoteFilesystems.Values.Select(fs => fs.RootINode.MTime).Max();
                long ctime = remoteFilesystems.Values.Select(fs => fs.RootINode.CTime).Max();
                return new VsRemoteINode(VsPath.ROOT, Enums.VsRemoteFileType.Directory, ctime, mtime);
            }
        }

        public ReadOnlyVirtualRoot(Dictionary<string, IVsRemoteFileSystem> remoteFilesystems)
        {
            this.remoteFilesystems = remoteFilesystems;
        }

        public Task CreateDirectory(string path)
        {
            throw new PermissionDenied();
        }

        public Task DeleteFile(string path)
        {
            throw new PermissionDenied();
        }

        public Task<IEnumerable<IVsRemoteINode>> ListDirectory(string path)
        {
            return Task.FromResult(remoteFilesystems.Select(fs => new VsRemoteINode(
                Name : fs.Key,
                FileType : VsRemoteFileType.Directory,
                CTime : fs.Value.RootINode.CTime,
                MTime : fs.Value.RootINode.MTime
            ) as IVsRemoteINode));
        }

        public Task<ReadOnlyMemory<byte>> ReadFile(string path)
        {
            throw new PermissionDenied();
        }

        public Task RemoveDirectory(string path, bool recursive)
        {
            throw new PermissionDenied();
        }

        public Task RenameFile(string fromPath, string toPath, bool overwrite)
        {
            throw new PermissionDenied();
        }

        public Task<IVsRemoteINode> Stat(string path)
        {
            if (VsPath.IsRoot(path))
            {
                return Task.FromResult(RootINode);
            }
            else
            {
                throw new IOError();
            }
        }

        public Task<long> WriteFile(string path, ReadOnlyMemory<byte> content, bool overwriteIfExists, bool createIfNotExists)
        {
            throw new PermissionDenied();
        }
    }
}
