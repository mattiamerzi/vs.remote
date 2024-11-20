namespace VsRemote.Exceptions;

public class PermissionDenied : VsException
{
    public const string ERROR_CODE = "E_PERM_DENIED";
    public override string ErrorCode => ERROR_CODE;

    public PermissionDenied() : base("Permission denied") { }

}
