namespace VsRemote.Exceptions;

public class NotEmpty: VsException
{
    public const string ERROR_CODE = "E_NOT_EMPTY";
    public override string ErrorCode => ERROR_CODE;

    public NotEmpty() : base("Directory not empty!") { }

}
