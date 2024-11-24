using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using VsRemote.Exceptions;
using VsRemote.Interfaces;
using VsRemote.Model.Auth;
using VsRemote.Utils;

namespace VsRemote.Services;

internal sealed class VsRemoteService : VsRemote.VsRemoteBase
{
    private readonly ILogger<VsRemoteService> logger;
    private readonly IVsRemoteFileSystemProvider remoteFsProvider;
    private readonly IVsRemoteAuthenticator remoteAuthenticator;
    private readonly IVsRemoteCommands remoteCommands;

    public VsRemoteService(ILogger<VsRemoteService> logger, IVsRemoteFileSystemProvider remoteFsProvider, IVsRemoteAuthenticator remoteAuthenticator, IVsRemoteCommands remoteCommands)
        => (this.logger, this.remoteFsProvider, this.remoteAuthenticator, this.remoteCommands) = (logger, remoteFsProvider, remoteAuthenticator, remoteCommands);

    public override async Task<ListCommandsResponse> ListCommands(ListCommandsRequest request, ServerCallContext context)
    {
        logger.LogDebug("ListCommands()");
        await ValidateToken(request.AuthToken);
        try
        {
            var response = new ListCommandsResponse();
            var commands = remoteCommands.GetCommands();
            if (commands.Any())
            {
                response.HasCommands = true;
                response.Commands.AddRange(
                    commands.Select(c => {
                        var cmd = new Command()
                        {
                            Name = c.Name,
                            Description = c.Description,
                            CommandTarget = c.Target.ToProtoBuf(),
                            ModifiesFileContent = c.CanChangeFile
                        };
                        cmd.Params.Add(c.Parameters.Select(p => new CommandParameter()
                        {
                            Name = p.Name,
                            Description = p.Description,
                            Validation = p.Validation.ToProtoBuf()
                        }));
                        return cmd;
                    })
                );
            }
            else
            {
                response.HasCommands = false;
            }
            logger.LogDebug("ListCommands() OK");
            return response;
        }
        catch(Exception ex)
        {
            logger.LogError("ListCommands() ERR: {err}", ex.Message);
            throw VsException.RpcFrom(ex);
        }
    }

    public override async Task<ExecuteCommandResponse> ExecuteCommand(ExecuteCommandRequest request, ServerCallContext context)
    {
        logger.LogDebug("ExecuteCommand({cmd})", request.Command);
        await ValidateToken(request.AuthToken);
        try
        {
            if (remoteCommands.TryGetCommand(request.Command, out IVsRemoteCommand? command))
            {
                var (RelativePath, RemoteFs) = remoteFsProvider.FromPath(request.Path, request.AuthToken);
                var result = await command.RunCommandAsync(request.AuthToken, RemoteFs, RelativePath, request.Params.ToDictionary(p => p.Name, p => p.Value));
                logger.LogDebug("ExecuteCommand({cmd}) OK", request.Command);
                return new ExecuteCommandResponse()
                {
                    Status = result.Success,
                    Message = result.Message
                };
            }
            else
            {
                throw new Exception($"no such command: {request.Command}");
            }
        }
        catch(Exception ex)
        {
            logger.LogError("ExecuteCommand({cmd}) ERR: {err}", request.Command, ex.Message);
            throw VsException.RpcFrom(ex);
        }
    }

    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        VsRemoteAuthenticateResult? authResult = null;
        if (!string.IsNullOrEmpty(request.AuthKey)) {
            logger.LogTrace("Login(auth_key)");
            authResult = await this.remoteAuthenticator.Authenticate(request.AuthKey);
        }
        else
        {
            if (!string.IsNullOrEmpty(request.Username))
            {
                logger.LogTrace("Login(username)");
                authResult = await this.remoteAuthenticator.Authenticate(request.Username, request.Password);
            }
        }

        if (authResult == null)
        {
            logger.LogError("Login() ERR: no authentication data provided");
            return new LoginResponse()
            {
                AuthResult = AuthResult.AuthenticationError,
                AuthToken = null,
                FailureMessage = "No authentication data provided"
            };
        }
        else
        {
            logger.LogDebug("Login() OK");
            return new LoginResponse()
            {
                AuthResult = authResult.AuthStatus.ToProtoBuf(),
                AuthToken = authResult.AuthToken,
                FailureMessage = authResult.FailureMessage
            };
        }
    }

    private async Task ValidateToken(string auth_token)
    {
        VsRemoteAuthenticationStatus status = await remoteAuthenticator.ValidateToken(auth_token);
        logger.LogTrace("ValidateToken(): {status}", status);
        if (status != VsRemoteAuthenticationStatus.AUTHENTICATED)
        {
            throw VsException.AuthenticationError(status);
        }
    }

    public override async Task<StatResponse> Stat(StatRequest request, ServerCallContext context)
    {
        logger.LogTrace("Stat({path})", request.Path);
        await ValidateToken(request.AuthToken);
        try
        {
            var (RelativePath, RemoteFs) = remoteFsProvider.FromPath(request.Path, request.AuthToken);
            var remoteInode = await RemoteFs.Stat(RelativePath);
            logger.LogDebug("Stat({path}) OK", request.Path);
            return new StatResponse()
            {
                FileInfo = remoteInode.ToGrpc()
            };
        }
        catch(Exception ex)
        {
            logger.LogError("Stat({path}) ERR: {err}", request.Path, ex.Message);
            throw VsException.RpcFrom(ex);
        }
    }

    public override async Task<ListDirectoryResponse> ListDirectory(ListDirectoryRequest request, ServerCallContext context)
    {
        logger.LogTrace("ListDirectory({path})", request.Path);
        await ValidateToken(request.AuthToken);
        try
        {
            var (RelativePath, RemoteFs) = remoteFsProvider.FromPath(request.Path, request.AuthToken);
            var remoteInodes = await RemoteFs.ListDirectory(RelativePath);
            var response = new ListDirectoryResponse();
            response.Entries.AddRange(remoteInodes.Select(i => i.ToGrpc()));
            logger.LogDebug("ListDirectory({path}) OK, {count} files.", request.Path, response.Entries.Count);
            return response;
        }
        catch(Exception ex)
        {
            logger.LogError("ListDirectory({path}) ERR: {err}", request.Path, ex.Message);
            throw VsException.RpcFrom(ex);
        }
    }

    public override async Task<CreateDirectoryResponse> CreateDirectory(CreateDirectoryRequest request, ServerCallContext context)
    {
        logger.LogTrace("CreateDirectory({path})", request.Path);
        await ValidateToken(request.AuthToken);
        try
        {
            var (RelativePath, RemoteFs) = remoteFsProvider.FromPath(request.Path, request.AuthToken);
            await RemoteFs.CreateDirectory(RelativePath);
            logger.LogDebug("CreateDirectory({path}) OK", request.Path);
            return new CreateDirectoryResponse();
        }
        catch(Exception ex)
        {
            logger.LogError("CreateDirectory({path}) ERR: {err}", request.Path, ex.Message);
            throw VsException.RpcFrom(ex);
        }
    }

    public override async Task<RemoveDirectoryResponse> RemoveDirectory(RemoveDirectoryRequest request, ServerCallContext context)
    {
        logger.LogTrace("RemoveDirectory({path}, Recursive:{recursive})", request.Path, request.Recursive);
        await ValidateToken(request.AuthToken);
        try
        {
            var (RelativePath, RemoteFs) = remoteFsProvider.FromPath(request.Path, request.AuthToken);
            await RemoteFs.RemoveDirectory(RelativePath, request.Recursive);
            logger.LogDebug("RemoveDirectory({path}) OK", request.Path);
            return new RemoveDirectoryResponse();
        }
        catch(Exception ex)
        {
            logger.LogError("RemoveDirectory({path}) ERR: {err}", request.Path, ex.Message);
            throw VsException.RpcFrom(ex);
        }
    }

    public override async Task<RenameFileResponse> RenameFile(RenameFileRequest request, ServerCallContext context)
    {
        logger.LogTrace("RenameFile({from} => {to}, Overwrite:{overwrite})", request.FromPath, request.ToPath, request.Overwrite);
        await ValidateToken(request.AuthToken);
        try
        {
            var (fromRelativePath, fromRemoteFs) = remoteFsProvider.FromPath(request.FromPath, request.AuthToken);
            var (toRelativePath, toRemoteFs) = remoteFsProvider.FromPath(request.ToPath, request.AuthToken);
            if (fromRemoteFs == toRemoteFs)
            {
                await toRemoteFs.RenameFile(fromRelativePath, toRelativePath, request.Overwrite);
            }
            else
            {
                if (!request.Overwrite)
                {
                    try
                    {
                        await toRemoteFs.Stat(toRelativePath);
                        throw new FileExists();
                    }
                    catch (FileNotFoundException) { }
                    catch { throw; }
                }
                var content = await fromRemoteFs.ReadFile(fromRelativePath);
                await toRemoteFs.WriteFile(toRelativePath, content, request.Overwrite, true);
                await fromRemoteFs.DeleteFile(fromRelativePath);
            }
            logger.LogDebug("RenameFile({from} => {to}) OK", request.FromPath, request.ToPath);
            return new RenameFileResponse();
        }
        catch(Exception ex)
        {
            logger.LogError("RenameFile({from} => {to}) ERR {err}", request.FromPath, request.ToPath, ex.Message);
            throw VsException.RpcFrom(ex);
        }
    }

    public override async Task<DeleteFileResponse> DeleteFile(DeleteFileRequest request, ServerCallContext context)
    {
        logger.LogTrace("DeleteFile({path})", request.Path);
        await ValidateToken(request.AuthToken);
        try
        {
            var (RelativePath, RemoteFs) = remoteFsProvider.FromPath(request.Path, request.AuthToken);
            await RemoteFs.DeleteFile(RelativePath);
            logger.LogDebug("DeleteFile({path}) OK", request.Path);
            return new DeleteFileResponse();
        }
        catch(Exception ex)
        {
            logger.LogError("DeleteFile({path}) ERR: {err}", request.Path, ex.Message);
            throw VsException.RpcFrom(ex);
        }
    }

    public override async Task<ReadFileResponse> ReadFile(ReadFileRequest request, ServerCallContext context)
    {
        logger.LogTrace("ReadFile({path})", request.Path);
        await ValidateToken(request.AuthToken);
        try
        {
            var (RelativePath, RemoteFs) = remoteFsProvider.FromPath(request.Path, request.AuthToken);
            var fileContent = await RemoteFs.ReadFile(RelativePath);
            logger.LogDebug("ReadFile({path}) OK, {length} bytes read", request.Path, fileContent.Length);
            return new ReadFileResponse()
            {
                Content = ByteString.CopyFrom(fileContent.Span),
                Length = fileContent.Length
            };
        }
        catch(Exception ex)
        {
            logger.LogError("ReadFile({path}) ERR: {err}", request.Path, ex.Message);
            throw VsException.RpcFrom(ex);
        }
    }

    public override async Task<ReadFileResponse> ReadFileOffset(ReadFileOffsetRequest request, ServerCallContext context)
    {
        logger.LogDebug("ReadFileOffset({path}, {offset}, {length})", request.Path, request.Offset, request.Length);
        await ValidateToken(request.AuthToken);
        try
        {
            var (RelativePath, RemoteFs) = remoteFsProvider.FromPath(request.Path, request.AuthToken);
            var fileContent = await RemoteFs.ReadFileOffset(RelativePath, request.Offset, request.Length);
            logger.LogDebug("ReadFileOffset({path}) OK, {length} bytes read", request.Path, fileContent.Length);
            return new ReadFileResponse()
            {
                Content = ByteString.CopyFrom(fileContent.Span),
                Length = fileContent.Length
            };
        }
        catch(Exception ex)
        {
            logger.LogError("ReadFileOffset({path}) ERR: {err}", request.Path, ex.Message);
            throw VsException.RpcFrom(ex);
        }
    }

    public override async Task<WriteFileResponse> CreateFile(CreateFileRequest request, ServerCallContext context)
    {
        logger.LogTrace("CreateFile({path})", request.Path);
        await ValidateToken(request.AuthToken);
        try
        {
            var (RelativePath, RemoteFs) = remoteFsProvider.FromPath(request.Path, request.AuthToken);
            await RemoteFs.CreateFile(RelativePath);
            logger.LogDebug("CreateFile({path}) OK", request.Path);
            return new WriteFileResponse()
            {
                BytesWritten = 0
            };
        }
        catch(Exception ex)
        {
            logger.LogError("WriteFile({path}) ERR: {err}", request.Path, ex.Message);
            throw VsException.RpcFrom(ex);
        }
    }

    public override async Task<WriteFileResponse> WriteFile(WriteFileRequest request, ServerCallContext context)
    {
        logger.LogDebug("WriteFile({path}, Create:{create}, Overwrite:{overwrite}, Buffer Length:{length})", request.Path, request.Create, request.Overwrite, request.Content.Length);
        await ValidateToken(request.AuthToken);
        try
        {
            var (RelativePath, RemoteFs) = remoteFsProvider.FromPath(request.Path, request.AuthToken);
            var bytesWritten = await RemoteFs.WriteFile(RelativePath, request.Content.Memory, request.Overwrite, request.Create);
            logger.LogDebug("WriteFile({path}) OK, {length} bytes written", request.Path, bytesWritten);
            return new WriteFileResponse()
            {
                BytesWritten = bytesWritten
            };
        }
        catch(Exception ex)
        {
            logger.LogError("WriteFile({path}) ERR: {err}", request.Path, ex.Message);
            throw VsException.RpcFrom(ex);
        }
    }

    public override async Task<WriteFileResponse> WriteFileOffset(WriteFileOffsetRequest request, ServerCallContext context)
    {
        logger.LogDebug("WriteFileOffset({path}, Offset:{offset}, Buffer Length:{length})", request.Path, request.Offset, request.Content.Length);
        await ValidateToken(request.AuthToken);
        try
        {
            var (RelativePath, RemoteFs) = remoteFsProvider.FromPath(request.Path, request.AuthToken);
            var bytesWritten = await RemoteFs.WriteFileOffset(RelativePath, request.Offset, request.Content.Memory);
            logger.LogDebug("WriteFileOffset({path}) OK, {length} bytes written", request.Path, bytesWritten);
            return new WriteFileResponse()
            {
                BytesWritten = bytesWritten
            };
        }
        catch(Exception ex)
        {
            logger.LogError("WriteFileOffset({path}) ERR: {err}", request.Path, ex.Message);
            throw VsException.RpcFrom(ex);
        }
    }

    public override async Task<WriteFileResponse> WriteFileAppend(WriteFileAppendRequest request, ServerCallContext context)
    {
        logger.LogTrace("WriteFileAppend({path}, Buffer Length:{length})", request.Path, request.Content.Length);
        await ValidateToken(request.AuthToken);
        try
        {
            var (RelativePath, RemoteFs) = remoteFsProvider.FromPath(request.Path, request.AuthToken);
            var bytesWritten = await RemoteFs.WriteFileAppend(RelativePath, request.Content.Memory);
            logger.LogDebug("WriteFileAppend({path}) OK, {length} bytes written", request.Path, bytesWritten);
            return new WriteFileResponse()
            {
                BytesWritten = bytesWritten
            };
        }
        catch(Exception ex)
        {
            logger.LogError("WriteFileAppend({path}) ERR: {err}", request.Path, ex.Message);
            throw VsException.RpcFrom(ex);
        }
    }
}