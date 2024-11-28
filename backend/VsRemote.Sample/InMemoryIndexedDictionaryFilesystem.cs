using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using System.Globalization;
using VsRemote.Base;
using VsRemote.Exceptions;
using VsRemote.Interfaces;
using VsRemote.Model;
using VsRemote.Enums;
using System.Text;
using System.Xml.Linq;

namespace VsRemote.Sample;

public class InMemoryIndexedDictionaryFilesystem : VsRemoteFileSystem<long>
{
    private static readonly IVsRemoteINode<long> _root = new VsRemoteRootINode<long>(
        Key: 1,
        CTime: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        MTime: DateTimeOffset.UtcNow.ToUnixTimeSeconds()
    );
    private readonly ConcurrentDictionary<long, IVsRemoteINode<long>> fs = new(new[] { new KeyValuePair<long, IVsRemoteINode<long>>(0, _root) });
    private readonly Dictionary<long, ReadOnlyMemory<byte>> contents = new();
    private long _nextId = 2;
    private long nextId => Interlocked.Increment(ref _nextId);

    public override IVsRemoteINode<long> RootINode => _root;

    public InMemoryIndexedDictionaryFilesystem()
    {
        var sampleContent = Encoding.ASCII.GetBytes("Sample file");
        CreateFile("sample.txt", _root, new ReadOnlyMemory<byte>(sampleContent));
    }

    public override Task CreateDirectory(string directoryName, IVsRemoteINode<long> parentDir)
    {
        IVsRemoteINode<long> newDir = new VsRemoteINode<long>(
            Key: nextId,
            Parent: parentDir.Key,
            Name: directoryName,
            FileType: VsRemoteFileType.Directory,
            CTime: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            MTime: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ATime: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Readonly: false
        );
        fs.AddOrUpdate(newDir.Key, newDir, (k, f) => throw new FileExists());
        return Task.CompletedTask;
    }

    public override Task<int> CreateFile(string file2write, IVsRemoteINode<long> parentDir, ReadOnlyMemory<byte> content)
    {
        IVsRemoteINode<long> newFile = new VsRemoteINode<long>(
            Key: nextId,
            Parent: parentDir.Key,
            Name: file2write,
            FileType: VsRemoteFileType.File,
            CTime: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            MTime: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ATime: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Readonly: false,
            Size: content.Length
        );
        fs.AddOrUpdate(newFile.Key, newFile, (k, f) => throw new FileExists());
        contents.Add(newFile.Key, content);
        return Task.FromResult(content.Length);
    }

    public override Task DeleteFile(IVsRemoteINode<long> fileToDelete)
    {
        fs.Remove(fileToDelete.Key, out _);
        contents.Remove(fileToDelete.Key);
        return Task.CompletedTask;
    }

    public override Task<IVsRemoteINode<long>> FindByName(string file, IVsRemoteINode<long> parentDir)
    {
        var fenum = fs.Values.Where(f => f.Parent == parentDir.Key && f.Name == file);
        if (fenum.Any())
            return Task.FromResult(fenum.First());
        else
            throw new NotFound();
    }

    public override Task<IEnumerable<IVsRemoteINode<long>>> ListDirectory(IVsRemoteINode<long> dir)
    {
        return Task.FromResult(fs.Values.Where(f => f.Parent == dir.Key));
    }

    public override Task<IVsRemoteINode<long>> RenameFile(IVsRemoteINode<long> file2rename, string toName)
    {
        if (file2rename.Name != toName)
        {
            var newFromFile = (file2rename as VsRemoteINode<long>)! with { Name = toName };
            if (!fs.TryUpdate(file2rename.Key, newFromFile, file2rename))
            {
                throw new NotFound();
            }
            return Task.FromResult((IVsRemoteINode<long>)newFromFile);
        }
        else
            return Task.FromResult(file2rename);
    }

    public override Task<IVsRemoteINode<long>> MoveFile(IVsRemoteINode<long> fromFile, IVsRemoteINode<long> toPath)
    {
        var newFromFile = (fromFile as VsRemoteINode<long>)! with { Parent = toPath.Key };
        if (!fs.TryUpdate(fromFile.Key, newFromFile, fromFile))
        {
            throw new NotFound();
        }
        return Task.FromResult((IVsRemoteINode<long>)newFromFile);
    }

    public override Task<ReadOnlyMemory<byte>> ReadFile(IVsRemoteINode<long> fileToRead)
    {
        if (contents.TryGetValue(fileToRead.Key, out var content))
        {
            return Task.FromResult(content);
        }
        else
        {
            throw new NotFound();
        }
    }

    public override Task RemoveDirectory(IVsRemoteINode<long> dir, bool recursive)
    {
        Subs(dir.Key);
        fs.Remove(dir.Key, out _);
        return Task.CompletedTask;

        void Subs(long dir)
        {
            foreach (var f in fs.Where(f => f.Value.Parent == dir))
            {
                if (f.Value.IsDirectory())
                    Subs(f.Value.Key);
                fs.Remove(f.Value.Key, out _);
            }
        }
    }

    public override Task<int> RewriteFile(IVsRemoteINode<long> file2rewrite, ReadOnlyMemory<byte> content)
    {
        contents[file2rewrite.Key] = content;
        fs[file2rewrite.Key] = (fs[file2rewrite.Key] as VsRemoteINode<long>)!with { Size = content.Length };
        return Task.FromResult(content.Length);
    }


}
