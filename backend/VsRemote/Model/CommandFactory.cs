using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VsRemote.Enums;
using VsRemote.Interfaces;

namespace VsRemote.Model;

public static class CommandFactory
{
    public static IVsRemoteCommand FromAction(Action action, string name, string? description = null)
        => new EmptyAction(name, description ?? name, action);

    public static IVsRemoteCommand FileOnlyAction(Action<IVsRemoteFileSystem, string> action, string name, string? description = null)
        => new FileOnlyAction(name, description ?? name, action);

    public static IVsRemoteCommand FromActionAsync(Func<Task> action, string name, string? description = null)
        => new EmptyActionAsync(name, description ?? name, action);

    public static IVsRemoteCommand FileOnlyActionAsync(Func<IVsRemoteFileSystem, string, Task> action, string name, string? description = null)
        => new FileOnlyActionAsync(name, description ?? name, action);

}

internal class EmptyAction: ResultFromExceptionCommand
{
    private readonly Action action;
    public override bool CanChangeFile => false;
    public override VsRemoteCommandTarget Target => VsRemoteCommandTarget.NONE;
    public EmptyAction(string name, string description, Action action): base(name, description)
        => this.action = action;

    protected override void ProtectedAction(string auth_token, IVsRemoteFileSystem remoteFs, string relativePath, Dictionary<string, string> parameters)
        => action();
}

internal class FileOnlyAction : ResultFromExceptionCommand
{
    private readonly Action<IVsRemoteFileSystem, string> action;
    public override bool CanChangeFile => true;
    public override VsRemoteCommandTarget Target => VsRemoteCommandTarget.FILE;
    public FileOnlyAction(string name, string description, Action<IVsRemoteFileSystem, string> action): base(name, description)
        => this.action = action;

    protected override void ProtectedAction(string auth_token, IVsRemoteFileSystem remoteFs, string relativePath, Dictionary<string, string> parameters)
        => action(remoteFs, relativePath);
}

internal abstract class ResultFromExceptionCommand : ParameterlessCommand
{
    protected ResultFromExceptionCommand(string name, string description) : base(name, description) { }

    protected abstract void ProtectedAction(string auth_token, IVsRemoteFileSystem remoteFs, string relativePath, Dictionary<string, string> parameters);

    public override Task<VsRemoteCommandResult> RunCommandAsync(string auth_token, IVsRemoteFileSystem remoteFs, string relativePath, Dictionary<string, string> parameters)
    {
        try
        {
            ProtectedAction(auth_token, remoteFs, relativePath, parameters);
            return Task.FromResult(new VsRemoteCommandResult(true, "OK"));
        }
        catch (Exception e)
        {
            return Task.FromResult(new VsRemoteCommandResult(false, e.Message));
        }
    }
}
internal class EmptyActionAsync: ResultFromExceptionCommandAsync
{
    private readonly Func<Task> action;
    public override bool CanChangeFile => false;
    public override VsRemoteCommandTarget Target => VsRemoteCommandTarget.NONE;
    public EmptyActionAsync(string name, string description, Func<Task> action): base(name, description)
        => this.action = action;

    protected override Task ProtectedActionAsync(string auth_token, IVsRemoteFileSystem remoteFs, string relativePath, Dictionary<string, string> parameters)
        => action();
}

internal class FileOnlyActionAsync : ResultFromExceptionCommandAsync
{
    private readonly Func<IVsRemoteFileSystem, string, Task> action;
    public override bool CanChangeFile => true;

    public override VsRemoteCommandTarget Target => VsRemoteCommandTarget.FILE;

    public FileOnlyActionAsync(string name, string description, Func<IVsRemoteFileSystem, string, Task> action): base(name, description)
        => this.action = action;

    protected override Task ProtectedActionAsync(string auth_token, IVsRemoteFileSystem remoteFs, string relativePath, Dictionary<string, string> parameters)
        => action(remoteFs, relativePath);
}

internal abstract class ResultFromExceptionCommandAsync : ParameterlessCommand
{
    protected ResultFromExceptionCommandAsync(string name, string description) : base(name, description) { }

    protected abstract Task ProtectedActionAsync(string auth_token, IVsRemoteFileSystem remoteFs, string relativePath, Dictionary<string, string> parameters);

    public override async Task<VsRemoteCommandResult> RunCommandAsync(string auth_token, IVsRemoteFileSystem remoteFs, string relativePath, Dictionary<string, string> parameters)
    {
        try
        {
            await ProtectedActionAsync(auth_token, remoteFs, relativePath, parameters);
            return new VsRemoteCommandResult(true, "OK");
        }
        catch (Exception e)
        {
            return new VsRemoteCommandResult(false, e.Message);
        }
    }
}
