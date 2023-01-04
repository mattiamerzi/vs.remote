using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using System.Globalization;
using Vs.Remote.Base;
using Vs.Remote.Exceptions;
using Vs.Remote.Interfaces;
using Vs.Remote.Model;
using Vs.Remote.Enums;
using System.Text;

namespace Vs.Remote.Sample;

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
            MTime: DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        );
        fs.AddOrUpdate(newDir.Key, newDir, (k, f) => throw new FileExists());
        return Task.CompletedTask;
    }

    public override Task<long> CreateFile(string file2write, IVsRemoteINode<long> parentDir, ReadOnlyMemory<byte> content)
    {
        IVsRemoteINode<long> newFile = new VsRemoteINode<long>(
            Key: nextId,
            Parent: parentDir.Key,
            Name: file2write,
            FileType: VsRemoteFileType.File,
            CTime: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            MTime: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Size: content.Length
        );
        fs.AddOrUpdate(newFile.Key, newFile, (k, f) => throw new FileExists());
        contents.Add(newFile.Key, content);
        return Task.FromResult((long)content.Length);
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

    public override Task OverwriteFile(IVsRemoteINode<long> fromFile, IVsRemoteINode<long> toFile)
    {
        var newFromFile = (fromFile as VsRemoteINode<long>)! with { Parent = toFile.Parent };
        if (fs.TryUpdate(newFromFile.Key, newFromFile, fromFile))
        {
            fs.Remove(toFile.Key, out _);
        }
        else
        {
            throw new NotFound();
        }
        return Task.CompletedTask;
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

    public override Task RenameFile(IVsRemoteINode<long> fromFile, string toName, IVsRemoteINode<long> toPath)
    {
        var newFromFile = (fromFile as VsRemoteINode<long>)! with { Name = toName, Parent = toPath.Parent };
        if (!fs.TryUpdate(newFromFile.Key, newFromFile, fromFile))
        {
            throw new NotFound();
        }
        return Task.CompletedTask;
    }

    public override Task<long> RewriteFile(IVsRemoteINode<long> file2rewrite, ReadOnlyMemory<byte> content)
    {
        contents[file2rewrite.Key] = content;
        return Task.FromResult((long)content.Length);
    }


}
