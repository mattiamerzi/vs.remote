using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using VsRemote.Services;

namespace VsRemote.Startup;

public static class VsRemoteIEndpointRouteBuilderExt
{

    public static IEndpointRouteBuilder MapVsRemote(this IEndpointRouteBuilder builder, Action<VsRemoteOptions>? configure = null)
    {
        VsRemoteOptions options = new();
        configure?.Invoke(options);

        builder.MapGrpcService<VsRemoteService>();

        if (options.EnableReflectionService)
            builder.MapGrpcReflectionService();

        builder.MapGet(options.RootFsPath, () =>
@"Vs.Remote gRPC interface should only be invoked by means of the Visual Studio Code dedicated extension.

If you are debugging Vs.Remote itself, please report any issue you might find on this project's github page!

Thank you for your hard work!

Mattia.");

        return builder;
    }

}
