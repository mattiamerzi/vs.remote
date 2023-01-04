namespace VsRemote.Exceptions;

public class NotADirectory: VsException
{
    public override string ErrorCode => "E_NOT_DIR";

    public NotADirectory() : base("Path is not a directory (and it should)") { }

}
