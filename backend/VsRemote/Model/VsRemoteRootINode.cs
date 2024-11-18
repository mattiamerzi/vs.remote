using VsRemote.Enums;
using VsRemote.Interfaces;
using VsRemote.Utils;

namespace VsRemote.Model;

public record VsRemoteRootINode(
    long CTime,
    long MTime
) : VsRemoteINode(VsPath.ROOT, VsRemoteFileType.Directory, CTime, MTime, 0);
