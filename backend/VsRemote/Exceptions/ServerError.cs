using Grpc.Core;

namespace VsRemote.Exceptions;

public class ServerError : VsException
{
    public const string ERROR_CODE = "E_ERROR";
    public override string ErrorCode => ERROR_CODE;

    public ServerError(Exception ex) : base($"Server Error: {ex.Message}", ex) { }

}
