using VsRemote.Enums;
using VsRemote.Model;

namespace VsRemote.Interfaces;

public interface IVsRemoteCommand
{
    public string Name { get; }
    public string Description { get; }
    public VsRemoteCommandTarget Target { get; }
    public bool CanChangeFile { get; }
    public IEnumerable<VsRemoteCommandParameter> Parameters { get; }
    public Task<VsRemoteCommandResult> RunCommandAsync(string auth_token, IVsRemoteFileSystem remoteFs, string relativePath, Dictionary<string, string> parameters);
}
