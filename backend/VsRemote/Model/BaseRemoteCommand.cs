using VsRemote.Interfaces;

namespace VsRemote.Model;

public abstract class BaseRemoteCommand : IVsRemoteCommand
{
    public string Name { get; init; }
    public string Description { get; init; }
    public abstract bool CanChangeFile { get; }

    public abstract IEnumerable<VsRemoteCommandParameter> Parameters { get; }

    public BaseRemoteCommand(string name, string description)
        => (Name, Description) = (name, description);

    public abstract Task<VsRemoteCommandResult> RunCommandAsync(string auth_token, IVsRemoteFileSystem remoteFs, string relativePath, Dictionary<string, string> parameters);

    protected virtual VsRemoteCommandResult Success(string? message = null)
        => new(true, message ?? "OK");

    protected virtual VsRemoteCommandResult Failure(string? message = null)
        => new(false, message ?? $"error executing command {Name}");

}
