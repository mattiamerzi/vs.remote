using Grpc.Core;
using Grpc.Net.Client;
using System.Collections.Concurrent;
using static VsRemote.VsRemote;

namespace VsRemote.Client;

public class VsRemoteStreamingClient: VsRemoteAuthenticatedBaseClient
{
    private readonly AsyncDuplexStreamingCall<DataRequest, DataResponse> streamFSstream;
    private readonly ConcurrentDictionary<int, TaskCompletionSource<DataResponse>> _pending = new();
    private int _nextRequestId = 0;
    private readonly Task _readerTask;

    public VsRemoteStreamingClient(GrpcChannel channel, string username, string password)
        : base(channel, username, password)
    {
        streamFSstream = this.StreamFS();
        _readerTask = Task.Run(ReadResponsesAsync);
    }
    public VsRemoteStreamingClient(GrpcChannel channel): base(channel)
    {
        streamFSstream = this.StreamFS();
        _readerTask = Task.Run(ReadResponsesAsync);
    }

    private int NextRequestId() => Interlocked.Increment(ref _nextRequestId);

    private async Task<DataResponse> SendRequest(DataRequest dataRequest)
    {
        int id = NextRequestId();
        dataRequest.RequestId = id;

        var tcs = new TaskCompletionSource<DataResponse>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        _pending[id] = tcs;

        await streamFSstream.RequestStream.WriteAsync(dataRequest);

        var resp = await tcs.Task;
        return resp;
    }

    private async Task ReadResponsesAsync()
    {
        try
        {
            await foreach (var resp in streamFSstream.ResponseStream.ReadAllAsync())
            {
                if (_pending.TryRemove(resp.RequestId, out var tcs))
                {
                    tcs.TrySetResult(resp);
                }
            }
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
        {
            // should be fine, stream was closed (gracefully?)
        }
        catch (Exception ex)
        {
            foreach (var kv in _pending)
                kv.Value.TrySetException(ex);
            _pending.Clear();
        }
    }

    public void Dispose()
    {
        streamFSstream.RequestStream.CompleteAsync().GetAwaiter().GetResult();
        _readerTask.GetAwaiter().GetResult();
        streamFSstream.Dispose();
    }

    #region Synchronous Methods

    public CreateDirectoryResponse CreateDirectory(CreateDirectoryRequest request)
        => CreateDirectoryAsync(request).GetAwaiter().GetResult();

    public StatResponse Stat(StatRequest request)
        => StatAsync(request).GetAwaiter().GetResult();

    public ListDirectoryResponse ListDirectory(ListDirectoryRequest request)
        => ListDirectoryAsync(request).GetAwaiter().GetResult();

    public RemoveDirectoryResponse RemoveDirectory(RemoveDirectoryRequest request)
        => RemoveDirectoryAsync(request).GetAwaiter().GetResult();

    public DeleteFileResponse DeleteFile(DeleteFileRequest request)
        => DeleteFileAsync(request).GetAwaiter().GetResult();

    public RenameFileResponse RenameFile(RenameFileRequest request)
        => RenameFileAsync(request).GetAwaiter().GetResult();

    public ReadFileResponse ReadFile(ReadFileRequest request)
        => ReadFileAsync(request).GetAwaiter().GetResult();

    public ReadFileResponse ReadFileOffset(ReadFileOffsetRequest request)
        => ReadFileOffsetAsync(request).GetAwaiter().GetResult();

    public WriteFileResponse CreateFile(CreateFileRequest request)
        => CreateFileAsync(request).GetAwaiter().GetResult();

    public WriteFileResponse WriteFile(WriteFileRequest request)
        => WriteFileAsync(request).GetAwaiter().GetResult();

    public WriteFileResponse WriteFileOffset(WriteFileOffsetRequest request)
        => WriteFileOffsetAsync(request).GetAwaiter().GetResult();

    public WriteFileResponse WriteFileAppend(WriteFileAppendRequest request)
        => WriteFileAppendAsync(request).GetAwaiter().GetResult();

    #endregion

    #region Asynchronous Methods

    public async Task<CreateDirectoryResponse> CreateDirectoryAsync(CreateDirectoryRequest request)
        => (await SendRequest(new DataRequest { CreateDirectory = request })).CreateDirectoryRes;

    public async Task<StatResponse> StatAsync(StatRequest request)
        => (await SendRequest(new DataRequest { Stat = request })).StatRes;

    public async Task<ListDirectoryResponse> ListDirectoryAsync(ListDirectoryRequest request)
        => (await SendRequest(new DataRequest { ListDirectory = request })).ListDirectoryRes;

    public async Task<RemoveDirectoryResponse> RemoveDirectoryAsync(RemoveDirectoryRequest request)
        => (await SendRequest(new DataRequest { RemoveDirectory = request })).RemoveDirectoryRes;

    public async Task<DeleteFileResponse> DeleteFileAsync(DeleteFileRequest request)
        => (await SendRequest(new DataRequest { DeleteFile = request })).DeleteFileRes;

    public async Task<RenameFileResponse> RenameFileAsync(RenameFileRequest request)
        => (await SendRequest(new DataRequest { RenameFile = request })).RenameFileRes;

    public async Task<ReadFileResponse> ReadFileAsync(ReadFileRequest request)
        => (await SendRequest(new DataRequest { ReadFile = request })).ReadFileRes;

    public async Task<ReadFileResponse> ReadFileOffsetAsync(ReadFileOffsetRequest request)
        => (await SendRequest(new DataRequest { ReadFileOffset = request })).ReadFileRes;

    public async Task<WriteFileResponse> CreateFileAsync(CreateFileRequest request)
        => (await SendRequest(new DataRequest { CreateFile = request })).WriteFileRes;

    public async Task<WriteFileResponse> WriteFileAsync(WriteFileRequest request)
        => (await SendRequest(new DataRequest { WriteFile = request })).WriteFileRes;

    public async Task<WriteFileResponse> WriteFileOffsetAsync(WriteFileOffsetRequest request)
        => (await SendRequest(new DataRequest { WriteFileOffset = request })).WriteFileRes;

    public async Task<WriteFileResponse> WriteFileAppendAsync(WriteFileAppendRequest request)
        => (await SendRequest(new DataRequest { WriteFileAppend = request })).WriteFileRes;

    #endregion
}
