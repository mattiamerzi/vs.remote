namespace Vs.Remote.Model.Auth;

public class VsRemoteAuthenticateResult
{
    private VsRemoteAuthenticateResult() { }

    public VsRemoteAuthenticationStatus AuthStatus { get; set; }

    private string? _authToken;
    public string AuthToken { get => _authToken ?? string.Empty; set => _authToken = value; }

    private string? _failureMessage;
    public string FailureMessage { get => _failureMessage ?? string.Empty; set => _failureMessage = value; }

    public static VsRemoteAuthenticateResult Authenticated(string authToken)
        => new() { AuthStatus = VsRemoteAuthenticationStatus.AUTHENTICATED, AuthToken = authToken };

    public static VsRemoteAuthenticateResult InvalidAuthKey
        => new() { AuthStatus = VsRemoteAuthenticationStatus.INVALID_AUTH_KEY, FailureMessage = "Invalid authentication key" };

    public static VsRemoteAuthenticateResult InvalidUsernameOrPassword
        => new() { AuthStatus = VsRemoteAuthenticationStatus.INVALID_USERNAME_OR_PASSWORD, FailureMessage = "Invalid username or password" };

    public static VsRemoteAuthenticateResult AuthenticationError(string errorMessage = "Authentication Error")
        => new() { AuthStatus = VsRemoteAuthenticationStatus.AUTHENTICATION_ERROR, FailureMessage = errorMessage };

}
