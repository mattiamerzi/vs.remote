using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using System.Text;
using VsRemote.Base;
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

    public BasePathFsProvider(Dictionary<string, IVsRemoteFileSystem> rootFss, Dictionary<string, string>? rootFiles = null)
        : this(rootFss, new Dictionary<string, BasePathFsProvider>()) { }
    public BasePathFsProvider(Dictionary<string, BasePathFsProvider> subMounts, Dictionary<string, string>? rootFiles = null)
        : this(null, subMounts) { }
    public BasePathFsProvider(Dictionary<string, IVsRemoteFileSystem>? rootFss, Dictionary<string, BasePathFsProvider> subMounts, Dictionary<string, string>? rootFiles = null)
    {
        rootFss ??= new();
        if (rootFss.Keys.Intersect(subMounts.Keys).Any())
            throw new InvalidPath("Duplicated mount point");
        foreach (var mountPoint in rootFss.Keys.Concat(subMounts.Keys))
        {
            if (VsPath.Split(mountPoint).Length != 1)
                throw new InvalidPath("Subdir filesystem mount point is not supported, only first level folders; use the second constructor paramter");
        }
        remoteFilesystems = rootFss;
        mountPoints = subMounts;
        virtualRootFs = new ReadOnlyVirtualRoot(remoteFilesystems, subMounts, new ReadonlyDictionaryFilesystem.BasicReadOnlyDictionaryFilesystem(rootFiles ?? new()));
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
                    return (path, virtualRootFs);
                }
            }
        }
    }

    private sealed class ReadOnlyVirtualRoot : VsRemoteFileSystem
    {
        private readonly Dictionary<string, IVsRemoteFileSystem> remoteFilesystems;
        private readonly Dictionary<string, BasePathFsProvider> mountPoints;
        private readonly ReadonlyDictionaryFilesystem rootFiles;

        public override IVsRemoteINode RootINode
        {
            get
            {
                long mtime =
                    remoteFilesystems.Values.Select(fs => fs.RootINode.MTime).Concat(
                    mountPoints.Values.Select(mp => mp.virtualRootFs.RootINode.MTime)).Max();
                long ctime =
                    remoteFilesystems.Values.Select(fs => fs.RootINode.CTime).Concat(
                    mountPoints.Values.Select(mp => mp.virtualRootFs.RootINode.CTime)).Max();
                long atime =
                    remoteFilesystems.Values.Select(fs => fs.RootINode.ATime).Concat(
                    mountPoints.Values.Select(mp => mp.virtualRootFs.RootINode.ATime)).Max();
                return new VsRemoteINode(VsPath.ROOT, VsRemoteFileType.Directory, ctime, mtime, atime);
            }
        }

        public ReadOnlyVirtualRoot(Dictionary<string, IVsRemoteFileSystem> remoteFilesystems, Dictionary<string, BasePathFsProvider> subMounts, ReadonlyDictionaryFilesystem rootFiles)
        {
            this.remoteFilesystems = remoteFilesystems;
            this.mountPoints = subMounts;
            this.rootFiles = rootFiles;
        }

        public override Task<IEnumerable<IVsRemoteINode>> ListDirectory(IVsRemoteINode dir, string[] path)
        {
            return Task.FromResult(
                remoteFilesystems.Select(fs => new VsRemoteINode(
                Name: fs.Key,
                FileType: VsRemoteFileType.Directory,
                CTime: fs.Value.RootINode.CTime,
                MTime: fs.Value.RootINode.MTime,
                ATime: fs.Value.RootINode.ATime
            ) as IVsRemoteINode).Concat(mountPoints.Select(mp => new VsRemoteINode(
                Name: mp.Key,
                FileType: VsRemoteFileType.Directory,
                CTime: mp.Value.virtualRootFs.RootINode.CTime,
                MTime: mp.Value.virtualRootFs.RootINode.ATime,
                ATime: mp.Value.virtualRootFs.RootINode.MTime
            ) as IVsRemoteINode).Concat(rootFiles.ListDirectory(dir, VsPath.ROOT_PATH).GetAwaiter().GetResult())));
        }

        public override Task<ReadOnlyMemory<byte>> ReadFile(IVsRemoteINode fileToRead, IVsRemoteINode parentDir, string[] parentPath)
            => rootFiles.ReadFile(fileToRead, parentDir, parentPath);

        public override Task<IVsRemoteINode> Stat(string[] path)
        {
            if (VsPath.IsRoot(path))
            {
                return Task.FromResult(RootINode);
            }
            else
            {
                return rootFiles.Stat(path);
            }
        }

        #region PermissionDenied methods
        public override Task CreateDirectory(string directoryName, IVsRemoteINode parentDir, string[] parentPath)
        {
            throw new PermissionDenied();
        }

        public override Task DeleteFile(IVsRemoteINode fileToDelete, IVsRemoteINode parentDir, string[] parentPath)
        {
            throw new PermissionDenied();
        }

        public override Task<int> WriteFile(string file2write, IVsRemoteINode parentDir, string[] parentPath, ReadOnlyMemory<byte> content)
        {
            throw new PermissionDenied();
        }

        public override Task RemoveDirectory(IVsRemoteINode dir, string[] path, bool recursive)
        {
            throw new PermissionDenied();
        }

        public override Task RenameFile(IVsRemoteINode fromFile, string[] fromPath, string toName, string[] toPath)
        {
            throw new PermissionDenied();
        }
#endregion

    }
}
