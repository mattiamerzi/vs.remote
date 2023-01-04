using System.Xml.Linq;
using Vs.Remote.Enums;
using Vs.Remote.Interfaces;
using Vs.Remote.Utils;

namespace Vs.Remote.Model;

public record VsRemoteRootINode<T> (
    T Key,
    long CTime,
    long MTime,
    T? Parent = default
) : VsRemoteINode<T>(Key, Parent, VsPath.ROOT, VsRemoteFileType.Directory, CTime, MTime, 0L), IVsRemoteINode<T>  where T : IEquatable<T>; 