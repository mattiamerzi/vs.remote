using VsRemote.Interfaces;

namespace VsRemote.Model.Auth;

public abstract class VsRemoteBaseAuthenticator : IVsRemoteAuthenticator
{
    private readonly VsRemoteAuthenticateResult _noauth =
        VsRemoteAuthenticateResult.AuthenticationError("Unsupported authentication scheme");

    public virtual Task<VsRemoteAuthenticateResult> Authenticate(string auth_key)
        => Task.FromResult(_noauth);

    public virtual Task<VsRemoteAuthenticateResult> Authenticate(string username, string password)
        => Task.FromResult(_noauth);

    public abstract Task<VsRemoteAuthenticationStatus> ValidateToken(string auth_token);

}
