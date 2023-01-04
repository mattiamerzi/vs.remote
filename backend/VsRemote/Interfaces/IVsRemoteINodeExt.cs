using VsRemote.Exceptions;
using VsRemote.Model;
using VsRemote.Utils;

namespace VsRemote.Interfaces;

public static class IVsRemoteINodeExt
{
    public static VsFsEntry ToGrpc(this IVsRemoteINode remoteInode) {
        return new VsFsEntry()
        {
            FileType = remoteInode.FileType.ToProtoBuf(),
            Name = remoteInode.Name,
            Ctime = remoteInode.CTime,
            Mtime = remoteInode.MTime
        };
    }

    public static bool IsDirectory(this IVsRemoteINode remoteInode)
        => remoteInode?.FileType == Enums.VsRemoteFileType.Directory;

    public static bool IsFile(this IVsRemoteINode remoteInode)
        => remoteInode?.FileType == Enums.VsRemoteFileType.File;

    public static IVsRemoteINode AssertDirectory(this IVsRemoteINode remoteINode)
    {
        if (!remoteINode.IsDirectory())
            throw new NotADirectory();
        return remoteINode;
    }

    public static IVsRemoteINode AssertNotDirectory(this IVsRemoteINode remoteINode)
    {
        if (remoteINode.IsDirectory())
            throw new IsADirectory();
        return remoteINode;
    }

    public static IVsRemoteINode<T> AssertDirectory<T>(this IVsRemoteINode<T> remoteINode) where T: IEquatable<T>
    {
        if (!remoteINode.IsDirectory())
            throw new NotADirectory();
        return remoteINode;
    }

    public static IVsRemoteINode<T> AssertNotDirectory<T>(this IVsRemoteINode<T> remoteINode) where T: IEquatable<T>
    {
        if (remoteINode.IsDirectory())
            throw new IsADirectory();
        return remoteINode;
    }
}
