using Grpc.Core;
using Vs.Remote.Model.Auth;

namespace Vs.Remote.Exceptions;

public abstract class VsException: Exception
{
    public abstract string ErrorCode { get; }
    public VsException(string message) : base(message) { }
    public VsException(string message, Exception innerException): base(message, innerException) { }


    public RpcException ToRpc()
    {
        return new RpcException(new Status(StatusCode.InvalidArgument, ErrorCode), new Metadata()
        {
            { "error_code", ErrorCode },
            { "error_message", Message }
        });
    }

    public static RpcException RpcFrom(Exception ex)
    {
        if (ex is RpcException rpcexception)
        {
            return rpcexception;
        }
        else
        {
            if (ex is VsException vsex)
            {
                return vsex.ToRpc();
            }
            else
            {
                return new ServerError(ex).ToRpc();
            }
        }
    }

    public static RpcException AuthenticationError(VsRemoteAuthenticationStatus status, string? message = null)
    {
        return new RpcException(new Status(StatusCode.Unauthenticated, status.ToString()), new Metadata()
        {
            { "error_code", status.ToString() },
            { "error_message", message ?? "Authentication error" }
        });
    }
}
