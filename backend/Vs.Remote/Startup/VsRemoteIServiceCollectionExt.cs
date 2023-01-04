using Grpc.AspNetCore.Server;
using Microsoft.Extensions.DependencyInjection;
using Vs.Remote.Interfaces;
using Vs.Remote.Model.Auth;

namespace Vs.Remote.Startup;

public static class VsRemoteIServiceCollectionExt
{
    public static IServiceCollection AddVsRemote(this IServiceCollection serviceCollection, IVsRemoteFileSystemProvider vsRemoteFileSystemProvider, IVsRemoteAuthenticator vsRemoteAuthenticator, Action<GrpcServiceOptions>? configureOptions = null)
        => InternalAddVsRemote(serviceCollection, vsRemoteFileSystemProvider, vsRemoteAuthenticator, configureOptions);

    public static IServiceCollection AddVsRemote(this IServiceCollection serviceCollection, IVsRemoteFileSystemProvider vsRemoteFileSystemProvider, Action<GrpcServiceOptions>? configureOptions = null)
        => InternalAddVsRemote(serviceCollection, vsRemoteFileSystemProvider, null, configureOptions);

    private static IServiceCollection InternalAddVsRemote(IServiceCollection serviceCollection, IVsRemoteFileSystemProvider vsRemoteFileSystemProvider, IVsRemoteAuthenticator? vsRemoteAuthenticator, Action<GrpcServiceOptions>? configureOptions)
    {
        serviceCollection.AddGrpc(grpcoptions =>
        {
            configOverride(grpcoptions);
        });

        serviceCollection.AddGrpcReflection();
        serviceCollection.AddSingleton<IVsRemoteFileSystemProvider>(vsRemoteFileSystemProvider);
        serviceCollection.AddSingleton<IVsRemoteAuthenticator>(vsRemoteAuthenticator ?? new VsRemoteNullAuthenticator());
        return serviceCollection;

        void configOverride(GrpcServiceOptions opt)
        {
            opt.MaxReceiveMessageSize = int.MaxValue;
            opt.MaxSendMessageSize = int.MaxValue;
            configureOptions?.Invoke(opt);
        }
    }

}
