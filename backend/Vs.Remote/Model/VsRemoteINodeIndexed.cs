using System.Xml.Linq;
using Vs.Remote.Enums;
using Vs.Remote.Interfaces;

namespace Vs.Remote.Model;

public record VsRemoteINode<T> (
    T Key,
    T? Parent,
    string Name,
    VsRemoteFileType FileType,
    long CTime,
    long MTime,
    long Size = 0
) : VsRemoteINode(Name, FileType, CTime, MTime, Size), IVsRemoteINode<T>  where T : IEquatable<T>; 