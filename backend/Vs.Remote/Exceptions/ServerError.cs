using Grpc.Core;

namespace Vs.Remote.Exceptions;

public class ServerError : VsException
{
    public override string ErrorCode => "E_ERROR";

    public ServerError(Exception ex) : base($"Server Error: {ex.Message}", ex) { }

}
