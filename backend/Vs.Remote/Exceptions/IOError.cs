namespace Vs.Remote.Exceptions;

public class IOError: VsException
{
    public override string ErrorCode => "E_IO_ERROR";

    public IOError() : base("I/O Error") { }

}
