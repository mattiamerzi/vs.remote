using Grpc.Net.Client;
using System.Runtime.CompilerServices;
using VsRemote.Interfaces;
using static VsRemote.VsRemote;

namespace VsRemote.Client;

public abstract class VsRemoteAuthenticatedBaseClient : VsRemoteClient
{
    protected readonly string? authToken;

    internal VsRemoteAuthenticatedBaseClient(GrpcChannel channel, string username, string password) : base(channel)
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
    internal VsRemoteAuthenticatedBaseClient(GrpcChannel channel) : base(channel)
    {
        this.authToken = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected T WithAuthToken<T>(T request) where T : IAuthenticatedRequest
    {
        if (authToken != null)
            request.AuthToken = authToken;
        return request;
    }
}

[Serializable]
public class VsRemoteAuthenticationException(string message) : Exception(message)
{
}
