namespace VsRemote.Exceptions;

public class InvalidPath : VsException
{
    public const string ERROR_CODE = "E_INVALID_PATH";
    public override string ErrorCode => ERROR_CODE;

    public InvalidPath() : base("Invalid path") { }
    public InvalidPath(string message) : base(message) { }
}
