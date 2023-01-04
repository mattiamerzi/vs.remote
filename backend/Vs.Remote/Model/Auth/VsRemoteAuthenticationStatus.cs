namespace Vs.Remote.Model.Auth;

public enum VsRemoteAuthenticationStatus
{
    AUTHENTICATED,
    INVALID_AUTH_KEY,
    INVALID_USERNAME_OR_PASSWORD,
    AUTHENTICATION_ERROR,
    EXPIRED
}
