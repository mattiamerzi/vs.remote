namespace VsRemote.Exceptions;

public class NotFound: VsException
{
    public const string ERROR_CODE = "E_NOT_FOUND";
    public override string ErrorCode => ERROR_CODE;

    public NotFound() : base("Path not found") { }

}
