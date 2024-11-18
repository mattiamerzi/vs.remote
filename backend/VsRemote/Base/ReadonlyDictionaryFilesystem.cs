using System.Diagnostics.CodeAnalysis;
using System.Text;
using VsRemote.Enums;
using VsRemote.Exceptions;
using VsRemote.Interfaces;
using VsRemote.Model;
using VsRemote.Utils;
using static System.Reflection.Metadata.BlobBuilder;

namespace VsRemote.Base;

public abstract class ReadonlyDictionaryFilesystem: VsRemoteFileSystem
{
    protected abstract IDictionary<string, string> Data { get; }
    protected virtual bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
        => Data.TryGetValue(key, out value);
    private IEnumerable<IVsRemoteINode> _dataINodes
        => Data.Select(f => new VsRemoteINode(f.Key, VsRemoteFileType.File, Epoch, Epoch, Epoch, f.Value.Length) as IVsRemoteINode);
    private readonly long epoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    protected virtual long Epoch => epoch;
    private readonly IVsRemoteINode _rootINode;

    protected ReadonlyDictionaryFilesystem()
    {
        _rootINode = new VsRemoteINode(VsPath.ROOT, VsRemoteFileType.Directory, Epoch, Epoch, Epoch);

    }

    public override IVsRemoteINode RootINode
        => _rootINode;


    public override Task<IEnumerable<IVsRemoteINode>> ListDirectory(IVsRemoteINode dir, string[] path)
    {
        if (dir.Name == VsPath.ROOT)
            return Task.FromResult(_dataINodes);
        else
            throw new NotFound();
    }

    public override Task<ReadOnlyMemory<byte>> ReadFile(IVsRemoteINode fileToRead, IVsRemoteINode parentDir, string[] parentPath)
    {
        if (parentDir.Name == VsPath.ROOT && TryGetValue(fileToRead.Name, out var data))
            return Task.FromResult(
                new ReadOnlyMemory<byte>(
                    Encoding.ASCII.GetBytes(data)
                )
            );
        throw new NotFound();
    }

    public override Task<IVsRemoteINode> Stat(string[] path)
    {
        switch (path.Length)
        {
            case 1:
                if (path[0] == VsPath.ROOT)
                    return Task.FromResult(_rootINode);
                else
                {
                    if (TryGetValue(path[0], out var data))
                        return Task.FromResult((IVsRemoteINode)
                            new VsRemoteINode(
                                path[0],
                                VsRemoteFileType.File,
                                Epoch, Epoch,
                                data.Length
                            )
                        );
                }
                break;
        }
        throw new NotFound();
    }

#region PermissionDenied methods
    public override Task CreateDirectory(string directoryName, IVsRemoteINode parentDir, string[] parentPath)
    {
        throw new PermissionDenied();
    }

    public override Task DeleteFile(IVsRemoteINode fileToDelete, IVsRemoteINode parentDir, string[] parentPath)
    {
        throw new PermissionDenied();
    }

    public override Task<int> WriteFile(string file2write, IVsRemoteINode parentDir, string[] parentPath, ReadOnlyMemory<byte> content)
    {
        throw new PermissionDenied();
    }

    public override Task RemoveDirectory(IVsRemoteINode dir, string[] path, bool recursive)
    {
        throw new PermissionDenied();
    }

    public override Task RenameFile(IVsRemoteINode fromFile, string[] fromPath, string toName, string[] toPath)
    {
        throw new PermissionDenied();
    }
#endregion

    public static ReadonlyDictionaryFilesystem FromDictionary(IDictionary<string, string> files)
        => new BasicReadOnlyDictionaryFilesystem(files);

    public class BasicReadOnlyDictionaryFilesystem : ReadonlyDictionaryFilesystem
    {
        protected override IDictionary<string, string> Data { get; }
        public BasicReadOnlyDictionaryFilesystem(IDictionary<string, string> data)
            => Data = data;
    }
}
