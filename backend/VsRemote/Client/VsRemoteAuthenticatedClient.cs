using Grpc.Net.Client;
using VsRemote.Interfaces;
using static VsRemote.VsRemote;

namespace VsRemote.Client;

public class VsRemoteAuthenticatedClient : VsRemoteClient
{
    private readonly string authToken;

    public VsRemoteAuthenticatedClient(GrpcChannel channel, string username, string password) : base(channel)
    {
        var res = Login(new LoginRequest() { Username = username, Password = password });
        if (res.AuthResult == AuthResult.Authenticated)
        {
            authToken = res.AuthToken;
        }
        else
        {
            throw new VsRemoteAuthenticationException(res.FailureMessage);
        }
    }

    private T WithAuthToken<T>(T request) where T : IAuthenticatedRequest
    {
        request.AuthToken = authToken;
        return request;
    }

    #region sync methods

    public CreateDirectoryResponse CreateDirectory(CreateDirectoryRequest request)
        => base.CreateDirectory(WithAuthToken(request));
    public WriteFileResponse CreateFile(CreateFileRequest request)
        => base.CreateFile(WithAuthToken(request));
    public DeleteFileResponse DeleteFile(DeleteFileRequest request)
        => base.DeleteFile(WithAuthToken(request));
    public ExecuteCommandResponse ExecuteCommand(ExecuteCommandRequest request)
        => base.ExecuteCommand(WithAuthToken(request));
    public ListCommandsResponse ListCommands(ListCommandsRequest request)
        => base.ListCommands(WithAuthToken(request));
    public ListDirectoryResponse ListDirectory(ListDirectoryRequest request)
        => base.ListDirectory(WithAuthToken(request));
    public ReadFileResponse ReadFile(ReadFileRequest request)
        => base.ReadFile(WithAuthToken(request));
    public ReadFileResponse ReadFileOffset(ReadFileOffsetRequest request)
        => base.ReadFileOffset(WithAuthToken(request));
    public RemoveDirectoryResponse RemoveDirectory(RemoveDirectoryRequest request)
        => base.RemoveDirectory(WithAuthToken(request));
    public RenameFileResponse RenameFile(RenameFileRequest request)
        => base.RenameFile(WithAuthToken(request));
    public StatResponse Stat(StatRequest request)
        => base.Stat(WithAuthToken(request));
    public WriteFileResponse WriteFile(WriteFileRequest request)
        => base.WriteFile(WithAuthToken(request));
    public WriteFileResponse WriteFileAppend(WriteFileAppendRequest request)
        => base.WriteFileAppend(WithAuthToken(request));
    public WriteFileResponse WriteFileOffset(WriteFileOffsetRequest request)
        => base.WriteFileOffset(WithAuthToken(request));
    #endregion

    #region async methods

    public async Task<CreateDirectoryResponse> CreateDirectoryAsync(CreateDirectoryRequest request)
        => await base.CreateDirectoryAsync(WithAuthToken(request));
    public async Task<WriteFileResponse> CreateFileAsync(CreateFileRequest request)
        => await base.CreateFileAsync(WithAuthToken(request));
    public async Task<DeleteFileResponse> DeleteFileAsync(DeleteFileRequest request)
        => await base.DeleteFileAsync(WithAuthToken(request));
    public async Task<ExecuteCommandResponse> ExecuteCommandAsync(ExecuteCommandRequest request)
        => await base.ExecuteCommandAsync(WithAuthToken(request));
    public async Task<ListCommandsResponse> ListCommandsAsync(ListCommandsRequest request)
        => await base.ListCommandsAsync(WithAuthToken(request));
    public async Task<ListDirectoryResponse> ListDirectoryAsync(ListDirectoryRequest request)
        => await base.ListDirectoryAsync(WithAuthToken(request));
    public async Task<ReadFileResponse> ReadFileAsync(ReadFileRequest request)
        => await base.ReadFileAsync(WithAuthToken(request));
    public async Task<ReadFileResponse> ReadFileOffsetAsync(ReadFileOffsetRequest request)
        => await base.ReadFileOffsetAsync(WithAuthToken(request));
    public async Task<RemoveDirectoryResponse> RemoveDirectoryAsync(RemoveDirectoryRequest request)
        => await base.RemoveDirectoryAsync(WithAuthToken(request));
    public async Task<RenameFileResponse> RenameFileAsync(RenameFileRequest request)
        => await base.RenameFileAsync(WithAuthToken(request));
    public async Task<StatResponse> StatAsync(StatRequest request)
        => await base.StatAsync(WithAuthToken(request));
    public async Task<WriteFileResponse> WriteFileAsync(WriteFileRequest request)
        => await base.WriteFileAsync(WithAuthToken(request));
    public async Task<WriteFileResponse> WriteFileAppendAsync(WriteFileAppendRequest request)
        => await base.WriteFileAppendAsync(WithAuthToken(request));
    public async Task<WriteFileResponse> WriteFileOffsetAsync(WriteFileOffsetRequest request)
        => await base.WriteFileOffsetAsync(WithAuthToken(request));

    #endregion
}

[Serializable]
public class VsRemoteAuthenticationException(string message) : Exception(message)
{
}
