namespace VsRemote.Exceptions;

public class FileExists: VsException
{
    public override string ErrorCode => "E_EXISTS";

    public FileExists() : base("File or directory already exists (and it shouldn't)") { }

}
