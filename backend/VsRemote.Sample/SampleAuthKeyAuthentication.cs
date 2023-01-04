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
    private static readonly ConcurrentDictionary<string, TokenStoreEntry> tokens = new();

    public override async Task<VsRemoteAuthenticateResult> Authenticate(string auth_key)
    {
        if (keys.Contains(auth_key))
        {
            string tmp = Guid.NewGuid().ToString();
            tokens.TryAdd(tmp, new(tmp));
            return VsRemoteAuthenticateResult.Authenticated(tmp);
        }
        else
        {
            return VsRemoteAuthenticateResult.InvalidAuthKey;
        }
    }

    public override async Task<VsRemoteAuthenticationStatus> ValidateToken(string auth_token)
    {
        foreach (var item in tokens.Where(t => t.Value.Expired).ToList())
        {
            tokens.TryRemove(item);
        }
        if (tokens.Where(t => t.Value.AuthToken == auth_token).Any())
            return VsRemoteAuthenticationStatus.AUTHENTICATED;
        else
            return VsRemoteAuthenticationStatus.EXPIRED;
    }

    private class TokenStoreEntry
    {
        public string AuthToken { get; init; }
        public DateTime ExpireTime { get; set; } = DateTime.UtcNow;
        public TokenStoreEntry(string authToken)
        {
            AuthToken = authToken;
        }
        public bool Expired => (DateTime.UtcNow - ExpireTime).TotalMinutes > TokenExpireTimeMinutes;
    }

}
