using Vs.Remote.Enums;
using Vs.Remote.Interfaces;

namespace Vs.Remote.Model;

public record VsRemoteINode (
    string Name,
    VsRemoteFileType FileType,
    long CTime,
    long MTime,
    long Size = 0
): IVsRemoteINode;
