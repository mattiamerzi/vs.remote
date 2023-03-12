using Grpc.AspNetCore.Server;
using Microsoft.Extensions.DependencyInjection;
using VsRemote.Interfaces;
using VsRemote.Model;
using VsRemote.Model.Auth;

namespace VsRemote.Startup;

public static class VsRemoteIServiceCollectionExt
{
    public static IServiceCollection AddVsRemote(this IServiceCollection serviceCollection, IVsRemoteFileSystemProvider vsRemoteFileSystemProvider, IVsRemoteAuthenticator vsRemoteAuthenticator, Action<GrpcServiceOptions>? configureOptions = null)
        => InternalAddVsRemote(serviceCollection, vsRemoteFileSystemProvider, vsRemoteAuthenticator, configureOptions);

    public static IServiceCollection AddVsRemote(this IServiceCollection serviceCollection, IVsRemoteFileSystemProvider vsRemoteFileSystemProvider, Action<GrpcServiceOptions>? configureOptions = null)
        => InternalAddVsRemote(serviceCollection, vsRemoteFileSystemProvider, null, configureOptions);

    private static IServiceCollection InternalAddVsRemote(IServiceCollection serviceCollection, IVsRemoteFileSystemProvider vsRemoteFileSystemProvider, IVsRemoteAuthenticator? vsRemoteAuthenticator, Action<GrpcServiceOptions>? configureOptions)
    {
        serviceCollection.AddGrpc(grpcoptions =>
            configOverride(grpcoptions)
        );

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

public class VsRemoteServiceBuilder
{
    internal IVsRemoteFileSystemProvider? VsRemoteFileSystemProvider;
    internal IVsRemoteAuthenticator? VsRemoteAuthenticator;
    internal VsRemoteCommands VsRemoteCommands = new VsRemoteCommands();

    public VsRemoteServiceBuilder SetRemoteFileSystemProvider(IVsRemoteFileSystemProvider vsRemoteFileSystemProvider)
    {
        VsRemoteFileSystemProvider = vsRemoteFileSystemProvider;
        return this;
    }

    public VsRemoteServiceBuilder SetAuthenticator(IVsRemoteAuthenticator vsRemoteAuthenticator)
    {
        VsRemoteAuthenticator = vsRemoteAuthenticator;
        return this;
    }

    public VsRemoteServiceBuilder AddCommand(IVsRemoteCommand vsRemoteCommand)
    {
        VsRemoteCommands.AddCommand(vsRemoteCommand);
        return this;
    }

}
