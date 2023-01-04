namespace VsRemote.Exceptions;

public class PermissionDenied : VsException
{
    public override string ErrorCode => "E_PERM_DENIED";

    public PermissionDenied() : base("Permission denied") { }

}
