namespace VsRemote.Exceptions;

public class IsADirectory: VsException
{
    public const string ERROR_CODE = "E_IS_DIR";
    public override string ErrorCode => ERROR_CODE;

    public IsADirectory() : base("Path is a directory (and it shouldn't)") { }

}
