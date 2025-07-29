using System.Collections.Concurrent;
using VsRemote.Model.Auth;

namespace VsRemote.Sample;

public class SampleAuthKeyAuthentication : VsRemoteBaseAuthenticator
{
    private const int TokenExpireTimeMinutes = 1;

    private static readonly ConcurrentBag<string> keys = new()
    {
        { "1234567890" },
        { "HelloWorld" },
        { "0000000000" }
    };
    private static readonly List<TokenStoreEntry> tokens = new();

    public override Task<VsRemoteAuthenticateResult> Authenticate(string auth_key)
    {
        if (keys.Contains(auth_key))
        {
            string tmp = Guid.NewGuid().ToString();
            lock (tokens)
            {
                tokens.Add(new(tmp));
            }
            return Task.FromResult(VsRemoteAuthenticateResult.Authenticated(tmp));
        }
        else
        {
            return Task.FromResult(VsRemoteAuthenticateResult.InvalidAuthKey);
        }
    }

    public override Task<VsRemoteAuthenticationStatus> ValidateToken(string auth_token)
    {
        lock (tokens)
        {
            tokens.RemoveAll(t => t.Expired);
            try
            {
                tokens.First(t => t.AuthToken == auth_token).ExpireTime = DateTime.UtcNow;
                return Task.FromResult(VsRemoteAuthenticationStatus.AUTHENTICATED);
            }
            catch
            {
                return Task.FromResult(VsRemoteAuthenticationStatus.EXPIRED);
            }
        }
    }

    private class TokenStoreEntry(string authToken)
    {
        public string AuthToken { get; init; } = authToken;
        public DateTime ExpireTime { get; set; } = DateTime.UtcNow;

        public bool Expired => (DateTime.UtcNow - ExpireTime).TotalMinutes > TokenExpireTimeMinutes;
    }

}
