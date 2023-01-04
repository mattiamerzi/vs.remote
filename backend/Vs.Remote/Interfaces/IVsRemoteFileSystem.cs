namespace Vs.Remote.Interfaces;

public interface IVsRemoteFileSystem
{
    public IVsRemoteINode RootINode { get; }

    public Task<IVsRemoteINode> Stat(string path);

    public Task<IEnumerable<IVsRemoteINode>> ListDirectory(string path);

    public Task CreateDirectory(string path);

    public Task RemoveDirectory(string path, bool recursive);

    public Task DeleteFile(string path);

    public Task RenameFile(string fromPath, string toPath, bool overwrite);

    public Task<ReadOnlyMemory<byte>> ReadFile(string path);

    public Task<long> WriteFile(string path, ReadOnlyMemory<byte> content, bool overwriteIfExists, bool createIfNotExists);
}
