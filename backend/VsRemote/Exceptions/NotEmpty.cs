namespace VsRemote.Exceptions;

public class NotEmpty: VsException
{
    public override string ErrorCode => "E_NOT_EMPTY";

    public NotEmpty() : base("Directory not empty!") { }

}
