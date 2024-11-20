namespace VsRemote.Exceptions;

public class FileExists: VsException
{
    public const string ERROR_CODE = "E_EXISTS";
    public override string ErrorCode => ERROR_CODE;

    public FileExists() : base("File or directory already exists (and it shouldn't)") { }

}
