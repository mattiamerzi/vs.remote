using VsRemote.Enums;

namespace VsRemote.Interfaces;

public interface IVsRemoteINode<T> : IVsRemoteINode where T : IEquatable<T>
{
    T Key { get; }
    T? Parent { get; }
}
