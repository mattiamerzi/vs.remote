namespace VsRemote.Exceptions;

public class IsADirectory: VsException
{
    public override string ErrorCode => "E_IS_DIR";

    public IsADirectory() : base("Path is a directory (and it shouldn't)") { }

}
