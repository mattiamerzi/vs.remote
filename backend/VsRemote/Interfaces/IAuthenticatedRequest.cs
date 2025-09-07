namespace VsRemote.Interfaces;

public interface IAuthenticatedRequest
{
    public string AuthToken { get; set; }
}
