using VsRemote.Enums;
using VsRemote.Exceptions;
using VsRemote.Interfaces;
using VsRemote.Model;
using VsRemote.Utils;

namespace VsRemote.Providers;

public class BasePathFsProvider : IVsRemoteFileSystemProvider
{
    private readonly Dictionary<string, IVsRemoteFileSystem> remoteFilesystems;
    private readonly Dictionary<string, BasePathFsProvider> mountPoints;
    private readonly IVsRemoteFileSystem virtualRootFs;

    public BasePathFsProvider(Dictionary<string, IVsRemoteFileSystem> rootFss)
        : this(rootFss, new Dictionary<string, BasePathFsProvider>()) { }
    public BasePathFsProvider(Dictionary<string, IVsRemoteFileSystem> rootFss, Dictionary<string, BasePathFsProvider> subMounts)
    {
        if (rootFss.Keys.Intersect(subMounts.Keys).Any())
            throw new InvalidPath("Duplicated mount point");
        foreach (var mountPoint in rootFss.Keys.Concat(subMounts.Keys))
        {
            if (VsPath.Split(mountPoint).Length != 1)
                throw new InvalidPath("Subdir filesystem mount point is not supported, only first level folders; use the second constructor paramter");
        }
        remoteFilesystems = rootFss;
        mountPoints = subMounts;
        virtualRootFs = new ReadOnlyVirtualRoot(remoteFilesystems, subMounts);
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
                if (mountPoints.TryGetValue(components[0], out BasePathFsProvider? subBasePath))
                {
                    return subBasePath.FromPath(VsPath.RemoveFirstDir(components), auth_token);
                }
                else
                {
                    throw new NotFound();
                }
            }
        }
    }

    private sealed class ReadOnlyVirtualRoot : IVsRemoteFileSystem
    {
        private readonly Dictionary<string, IVsRemoteFileSystem> remoteFilesystems;
    private readonly Dictionary<string, BasePathFsProvider> mountPoints;

        public IVsRemoteINode RootINode
        {
            get
            {
                long mtime =
                    remoteFilesystems.Values.Select(fs => fs.RootINode.MTime).Concat(
                    mountPoints.Values.Select(mp => mp.virtualRootFs.RootINode.MTime)).Max();
                long ctime =
                    remoteFilesystems.Values.Select(fs => fs.RootINode.CTime).Concat(
                    mountPoints.Values.Select(mp => mp.virtualRootFs.RootINode.CTime)).Max();
                return new VsRemoteINode(VsPath.ROOT, Enums.VsRemoteFileType.Directory, ctime, mtime);
            }
        }

        public ReadOnlyVirtualRoot(Dictionary<string, IVsRemoteFileSystem> remoteFilesystems, Dictionary<string, BasePathFsProvider> subMounts)
        {
            this.remoteFilesystems = remoteFilesystems;
            this.mountPoints = subMounts;
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
            ) as IVsRemoteINode).Concat(mountPoints.Select(mp => new VsRemoteINode(
                Name : mp.Key,
                FileType : VsRemoteFileType.Directory,
                CTime : mp.Value.virtualRootFs.RootINode.CTime,
                MTime : mp.Value.virtualRootFs.RootINode.MTime
            ))));
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
