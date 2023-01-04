using VsRemote.Enums;

namespace VsRemote.Utils;

public static class FileTypeExt
{
    public static FileType ToProtoBuf(this VsRemoteFileType vsRemoteFileType)
        => vsRemoteFileType switch
            {
                VsRemoteFileType.Unknown => FileType.Unknown,
                VsRemoteFileType.File => FileType.File,
                VsRemoteFileType.Directory => FileType.Directory,
                VsRemoteFileType.SymbolicLink => FileType.SymbolicLink,
                _ => FileType.Unknown
            };
}
