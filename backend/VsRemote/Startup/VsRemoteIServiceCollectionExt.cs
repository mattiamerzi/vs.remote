using Grpc.AspNetCore.Server;
using Microsoft.Extensions.DependencyInjection;
using VsRemote.Exceptions;
using VsRemote.Interfaces;
using VsRemote.Model;
using VsRemote.Model.Auth;

namespace VsRemote.Startup;

public static class VsRemoteIServiceCollectionExt
{
    public static IServiceCollection AddVsRemote(this IServiceCollection serviceCollection, IVsRemoteFileSystemProvider vsRemoteFileSystemProvider, IVsRemoteAuthenticator vsRemoteAuthenticator, Action<GrpcServiceOptions>? configureOptions = null)
        => AddVsRemote(serviceCollection, b =>
            {
                b.VsRemoteFileSystemProvider = vsRemoteFileSystemProvider;
                b.VsRemoteAuthenticator = vsRemoteAuthenticator;
                b.SetGrpcServiceOptions(configureOptions);
            });

    public static IServiceCollection AddVsRemote(this IServiceCollection serviceCollection, IVsRemoteFileSystemProvider vsRemoteFileSystemProvider, Action<GrpcServiceOptions>? configureOptions = null)
        => AddVsRemote(serviceCollection, b =>
            {
                b.VsRemoteFileSystemProvider = vsRemoteFileSystemProvider;
                b.SetGrpcServiceOptions(configureOptions);
            });

    public static IServiceCollection AddVsRemote(this IServiceCollection serviceCollection, Action<VsRemoteServiceBuilder> configureVsRemote)
    {
        VsRemoteServiceBuilder builder = new();
        configureVsRemote(builder);
        InternalAddVsRemote(serviceCollection, builder);
        return serviceCollection;
    }

    private static IServiceCollection InternalAddVsRemote(IServiceCollection serviceCollection, VsRemoteServiceBuilder builder)
    {
        if (builder.VsRemoteFileSystemProvider == null)
            throw new ConfigurationException("Missing VsRemoteFileSystemProvider");
        serviceCollection.AddGrpc(grpcoptions =>
            configOverride(grpcoptions)
        );

        serviceCollection.AddGrpcReflection();
        serviceCollection.AddSingleton<IVsRemoteCommands>(builder.VsRemoteCommands);
        serviceCollection.AddSingleton<IVsRemoteFileSystemProvider>(builder.VsRemoteFileSystemProvider);
        serviceCollection.AddSingleton<IVsRemoteAuthenticator>(builder.VsRemoteAuthenticator);
        return serviceCollection;

        void configOverride(GrpcServiceOptions opt)
        {
            opt.MaxReceiveMessageSize = int.MaxValue;
            opt.MaxSendMessageSize = int.MaxValue;
            builder.configureOptions?.Invoke(opt);
        }
    }

}

public class VsRemoteServiceBuilder
{
    internal IVsRemoteFileSystemProvider? VsRemoteFileSystemProvider;
    internal IVsRemoteAuthenticator VsRemoteAuthenticator = new VsRemoteNullAuthenticator();
    internal VsRemoteCommands VsRemoteCommands = new();
    internal Action<GrpcServiceOptions>? configureOptions = null;

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

    public VsRemoteServiceBuilder SetGrpcServiceOptions(Action<GrpcServiceOptions>? configureOptions)
    {
        this.configureOptions = configureOptions;
        return this;
    }
}
