﻿using System.Xml.Linq;
using VsRemote.Enums;
using VsRemote.Interfaces;

namespace VsRemote.Model;

public record VsRemoteINode<T> (
    T Key,
    T? Parent,
    string Name,
    VsRemoteFileType FileType,
    long CTime,
    long MTime,
    long Size = 0
) : VsRemoteINode(Name, FileType, CTime, MTime, Size), IVsRemoteINode<T>  where T : IEquatable<T>; 