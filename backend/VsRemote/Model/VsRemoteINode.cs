using VsRemote.Enums;
using VsRemote.Interfaces;

namespace VsRemote.Model;

public record VsRemoteINode (
    string Name,
    VsRemoteFileType FileType,
    long CTime,
    long MTime,
    long Size = 0
): IVsRemoteINode;
