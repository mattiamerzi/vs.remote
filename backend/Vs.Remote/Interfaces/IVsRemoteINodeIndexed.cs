using Vs.Remote.Enums;

namespace Vs.Remote.Interfaces;

public interface IVsRemoteINode<T> : IVsRemoteINode where T : IEquatable<T>
{
    T Key { get; }
    T? Parent { get; }
}
