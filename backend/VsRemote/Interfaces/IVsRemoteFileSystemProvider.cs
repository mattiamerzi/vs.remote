namespace VsRemote.Interfaces;

public interface IVsRemoteFileSystemProvider
{
    public (string RelativePath, IVsRemoteFileSystem RemoteFs) FromPath(string path, string? auth_token);

}
