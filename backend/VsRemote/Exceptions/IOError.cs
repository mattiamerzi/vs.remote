namespace VsRemote.Exceptions;

public class IOError: VsException
{
    public const string ERROR_CODE = "E_IO_ERROR";
    public override string ErrorCode => ERROR_CODE;

    public IOError() : base("I/O Error") { }

}
