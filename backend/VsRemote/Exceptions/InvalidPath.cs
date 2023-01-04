namespace VsRemote.Exceptions;

public class InvalidPath : VsException
{
    public override string ErrorCode => "E_INVALID_PATH";

    public InvalidPath() : base("Invalid path") { }
    public InvalidPath(string message) : base(message) { }
}
