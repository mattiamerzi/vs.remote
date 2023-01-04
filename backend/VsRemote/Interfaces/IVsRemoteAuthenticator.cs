using VsRemote.Model.Auth;

namespace VsRemote.Interfaces;

public interface IVsRemoteAuthenticator
{
    public Task<VsRemoteAuthenticateResult> Authenticate(string auth_key);

    public Task<VsRemoteAuthenticateResult> Authenticate(string username, string password);

    public Task<VsRemoteAuthenticationStatus> ValidateToken(string auth_token);

}
