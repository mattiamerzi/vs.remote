using VsRemote.Enums;

namespace VsRemote.Interfaces;

public interface IVsRemoteINode
{
    string Name { get; }
    VsRemoteFileType FileType { get; }
    long CTime { get; }
    long MTime { get; }
    long ATime { get; }
    long Size { get; }
}
