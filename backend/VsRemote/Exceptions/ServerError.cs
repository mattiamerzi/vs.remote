using Grpc.Core;

namespace VsRemote.Exceptions;

public class ServerError : VsException
{
    public override string ErrorCode => "E_ERROR";

    public ServerError(Exception ex) : base($"Server Error: {ex.Message}", ex) { }

}
