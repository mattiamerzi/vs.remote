namespace Vs.Remote.Exceptions;

public class NotFound: VsException
{
    public override string ErrorCode => "E_NOT_FOUND";

    public NotFound() : base("Path not found") { }

}
