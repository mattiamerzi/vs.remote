namespace VsRemote.Exceptions;

public class NotADirectory: VsException
{
    public const string ERROR_CODE = "E_NOT_DIR";
    public override string ErrorCode => ERROR_CODE;

    public NotADirectory() : base("Path is not a directory (and it should)") { }

}
