using System.Xml.Linq;
using VsRemote.Enums;
using VsRemote.Interfaces;
using VsRemote.Utils;

namespace VsRemote.Model;

public record VsRemoteRootINode<T> (
    T Key,
    long CTime,
    long MTime,
    T? Parent = default
) : VsRemoteINode<T>(Key, Parent, VsPath.ROOT, VsRemoteFileType.Directory, Readonly: false, CTime, MTime, 0L), IVsRemoteINode<T>  where T : IEquatable<T>; 