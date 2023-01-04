using VsRemote.Interfaces;

namespace VsRemote.Providers;

public class RootFsProvider : IVsRemoteFileSystemProvider
{
    private readonly IVsRemoteFileSystem RootFs;

    public RootFsProvider(IVsRemoteFileSystem rootFs)
        => RootFs = rootFs;

    public (string RelativePath, IVsRemoteFileSystem RemoteFs) FromPath(string path, string? auth_token)
        => (RelativePath: path, RemoteFs: RootFs);

}
