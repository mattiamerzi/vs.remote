using Vs.Remote.Enums;
using Vs.Remote.Interfaces;
using Vs.Remote.Utils;

namespace Vs.Remote.Model;

public record VsRemoteRootINode(
    long CTime,
    long MTime
) : VsRemoteINode(VsPath.ROOT, VsRemoteFileType.Directory, CTime, MTime, 0), IVsRemoteINode;
