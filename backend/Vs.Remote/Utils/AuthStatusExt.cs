using Vs.Remote.Model.Auth;

namespace Vs.Remote.Utils;

public static class AuthStatusExt
{
    public static AuthResult ToProtoBuf(this VsRemoteAuthenticationStatus vsRemoteAuthStatus)
        => vsRemoteAuthStatus switch
            {
                VsRemoteAuthenticationStatus.AUTHENTICATED => AuthResult.Authenticated,
                VsRemoteAuthenticationStatus.INVALID_AUTH_KEY => AuthResult.InvalidAuthKey,
                VsRemoteAuthenticationStatus.INVALID_USERNAME_OR_PASSWORD => AuthResult.InvalidUsernameOrPassword,
                VsRemoteAuthenticationStatus.EXPIRED => AuthResult.Expired,
                VsRemoteAuthenticationStatus.AUTHENTICATION_ERROR => AuthResult.AuthenticationError,
                _ => AuthResult.AuthenticationError
            };
}
