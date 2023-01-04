using VsRemote.Interfaces;

namespace VsRemote.Model.Auth;

public class VsRemoteNullAuthenticator : IVsRemoteAuthenticator
{
    private readonly VsRemoteAuthenticateResult _auth = VsRemoteAuthenticateResult.Authenticated(string.Empty);

    public virtual Task<VsRemoteAuthenticateResult> Authenticate(string auth_key)
        => Task.FromResult(_auth);

    public virtual Task<VsRemoteAuthenticateResult> Authenticate(string username, string password)
        => Task.FromResult(_auth);

    public Task<VsRemoteAuthenticationStatus> ValidateToken(string auth_token)
        => Task.FromResult(VsRemoteAuthenticationStatus.AUTHENTICATED);

}
