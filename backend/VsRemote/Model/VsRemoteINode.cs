using VsRemote.Enums;
using VsRemote.Interfaces;

namespace VsRemote.Model;

public record VsRemoteINode (
    string Name,
    VsRemoteFileType FileType,
    bool Readonly,
    long CTime,
    long MTime,
    long ATime,
    long Size = 0
): IVsRemoteINode;
