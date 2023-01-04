using Vs.Remote.Enums;

namespace Vs.Remote.Interfaces;

public interface IVsRemoteINode
{
    string Name { get; }
    VsRemoteFileType FileType { get; }
    long CTime { get; }
    long MTime { get; }
    long Size { get; }
}
