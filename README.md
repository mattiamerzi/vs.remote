![GitHub](https://img.shields.io/github/license/mattiamerzi/vs.remote)
![GitHub issues](https://img.shields.io/github/issues/mattiamerzi/vs.remote)
![Code Climate maintainability](https://img.shields.io/codeclimate/maintainability/mattiamerzi/vs.remote)
![AppVeyor](https://img.shields.io/appveyor/build/mattiamerzi/vs-remote)
![Nuget](https://img.shields.io/nuget/v/VsRemote)

# Vs.Remote
## A Visual Studio Code remote filesystem implementation with dotnet core backend, based on protocol buffers, gRPC and asp.net

Vs.Remote allows you to connect your Visual Studio Code workspace to a generic remote filesystem, where the backend is based on a custom dotnet core implementation.

You can store your files wherever you need them to be: on a database, in memory, on a real filesystem ...

**The backend package is [available on nuget.org as VsRemote](https://www.nuget.org/packages/VsRemote/), while the vscode extension must be installed manually; you can grab your .vsix extension file [HERE](https://github.com/mattiamerzi/vs.remote/releases/tag/pre-release)**



## Quick Start
This example has been created using Visual Studio 2022 Community and Visual Studio Code 1.74.2 (*user setup*); your mileage may vary.

Create an empty .net 6+ **ASP.NET Core Web API** project named **TestVsRemote**

<img src="https://i.ibb.co/VptBmTN/new-vs-project.png" alt="new-vs-project" border="0">

Uncheck all the various checkboxes and set authentication type to "None", this should provide you with the simplest project possible.

<img src="https://i.ibb.co/rv4d275/vs-project-config.png" alt="vs-project-config" border="0">

Add the "VsRemote" nuget package to the project; in the nuget search options you should check the "Include prerelease" checkbox.

<img src="https://i.ibb.co/D8x1wCB/add-nuget-pkg.png" alt="add-nuget-pkg" border="0">

Import the sample in-memory filesystem into your project from this URL:

[InMemoryIndexedDictionaryFilesystem.cs](https://raw.githubusercontent.com/mattiamerzi/vs.remote/main/backend/VsRemote.Sample/InMemoryIndexedDictionaryFilesystem.cs)

and place it into the root folder of your project.

Open Program.cs, remove all the code that's been generated inside and copy paste this code:

    using VsRemote.Interfaces;
    using VsRemote.Providers;
    using VsRemote.Sample;
    using VsRemote.Startup;
    
    var builder = WebApplication.CreateBuilder(args);
    IVsRemoteFileSystemProvider fsProvider =
        new RootFsProvider(
            new InMemoryIndexedDictionaryFilesystem());
    builder.Services.AddVsRemote(fsProvider);
    var app = builder.Build();
    app.MapVsRemote();
    app.Run();

Open appsettings.json and update the content with this:

    {
      "Logging": {
        "LogLevel": {
          "Default": "Debug",
          "Microsoft.AspNetCore": "Warning"
        },
        "Console": {
          "FormatterName": "Simple",
          "FormatterOptions": {
            "SingleLine":  true
          },
          "LogLevel": {
            "Default": "Debug",
            "Microsoft": "Warning",
            "Grpc": "Warning"
          }
        }
      },
      "AllowedHosts": "*",
      "Kestrel": {
        "EndpointDefaults": {
          "Protocols": "Http2"
        }
      }
    }

this basically enables Http2 on Kestrel and sets the interesting logs log level to "Debug".

Open Properties/launchSettings.json and update the content with this:

    {
      "profiles": {
        "TestVsRemote": {
          "commandName": "Project",
          "dotnetRunMessages": true,
          "launchBrowser": false,
          "applicationUrl": "http://localhost:5090",
          "environmentVariables": {
            "ASPNETCORE_ENVIRONMENT": "Development"
          }
        }
      }
    }

Grab the Visual Studio Code extension file from here:
[vsremote-0.0.1.vsix](https://github.com/mattiamerzi/vs.remote/releases/download/pre-release/vsremote-0.0.1.vsix)

Open Visual Studio code, go to the Extensions tab and manually install the extension:

<img src="https://i.ibb.co/TTKTBH9/vscode-install-vsix.png" alt="vscode-install-vsix" border="0">

Go to File => Preferences => Settings and open the json settings file clicking the "Open Settings (JSON)" icon on the top right corner:

<img src="https://i.ibb.co/PY2X6qz/open-settings-json.png" alt="open-settings-json" border="0">

Append this json block to the end of the file:

    "vs.remote": {
        "remotes": {
            "test2fs": {
                "host": "localhost",
                "port": 5090
            }
        }
    }

<img src="https://i.ibb.co/r7JGrkx/edit-settings-json-file.png" alt="edit-settings-json-file" border="0">

Save and close.
Now, **assuming the backend project is still running**, open the commands dialog ( SHIFT + CTRL + P ) and search for Vs.Remote: Add Vs.Remote folder to Workspace

<img src="https://i.ibb.co/xYZ2cnm/vsremote-command.png" alt="vsremote-command" border="0">

launch that command, choose the only option that will appear in the next dialog

<img src="https://i.ibb.co/r09PnGS/test2fs-command.png" alt="test2fs-command" border="0">

now, in your vscode workspace there should be a new folder containing a single sample.txt file

<img src="https://i.ibb.co/BzG2xKX/vsremote-folder.png" alt="vsremote-folder" border="0">

That's it! Visual Studio Code is connected to your sample backend project, and the files you will create or store will reside into the sample in-memory filesystem you have just created.
In the next sections you'll find more details on how to create a filesystem and adapt the backend to your needs. If you find any trouble working with Vs.Remote, feel free to open an issue on github.



## Create your own filesystem

Depending on how your backend works, you can choose to implement your filesystem as path-based or index-based.
If your backend provides some "keys" that can be used to obtain a direct reference to a file or folder, then you should proceed with a key-based approach. The key can be of any type, as long as it implements the `IEquatable<T>` interface.

On the contrary, if your backend has no "keys" ready to be used, you need to rely on a path-based approach.

A key-based filesystem should be created extending the `VsRemoteFileSystem<T>` abstract class, providing a type for the `Key`; e.g.:

    public class ExampleRelationalDatabaseBasedFilesystem : VsRemoteFileSystem<long>

A path-based filesystem, instead, should be created extending the `VsRemoteFileSystem` abstract class.

Although these two abstract classes provide you most of the base validation, exception handling, and various other checks already made, if you need a more fine-grained and maybe a more efficient solution you can implement either the `IVsRemoteFileSystem` or `IVsRemoteFileSystem<T>` interfaces and write all the boring stuff by yourself.

If you need any more detail, please refer to the examples and classes documentation.

After having decided which of these approaches best fits your needs, you can proceed with the tricky part: implementing the filesystem APIs.

If you have chosen a path-based approach, the APIs will receive their parameters based on the full path to the files, split into string arrays, plus an `IVsRemoteINode`, that is a file descriptor with this structure:

    public interface IVsRemoteINode
    {
        string Name { get; } // the name of the file
        VsRemoteFileType FileType { get; } // the file type (only File or Directory, atm)
        long CTime { get; } // create time (unix timestamp)
        long MTime { get; } // last modify time (unix timestamp)
        long Size { get; } // size in bytes (0 for directories)
    }

for example:

    public abstract Task CreateDirectory(string directoryName, IVsRemoteINode parentDir, string[] parentPath);

If you have chosen a key-based approach instead, the APIs will receive their parameters based on the `IVsRemoteINode<T>` class; this class adds to the `IVsRemoteINode` the `Key` and `Parent` properties of type T, for example:

    public abstract Task CreateDirectory(string directoryName, IVsRemoteINode<T> parentDir);

As you can see, the full path to the directory is missing, hence you shouldn't rely on that on your key-based implementation.

Whenever you need to create a `IVsRemoteINode` instance you can instantiate an object of type `VsRemoteINode` (or `VsRemoteINode<T>`), that is the default implementation; for the special case of the root directory, there is a `VsRemoteRootINode` facility.



## Mounting the filesystems

The Vs.Remote subsystem requires the user to provide an instance of `IVsRemoteFileSystemProvider`, that is, a factory that maps paths to `IVsRemoteFilesystem` implementations.

This allows you, for example, to expose multiple filesystems on a single interface.

This interface has a single method `FromPath` whose logic is: given a path and optionally an authentication token (more on this later), a tuple containing a particular filesystem and the path you just provided, relativized on the returned filesystem's path will be returned.

    public (string RelativePath, IVsRemoteFileSystem RemoteFs) FromPath(string path, string? auth_token);

**Bundled with Vs.Remote you'll find two different implementations of filesystem provider: `RootFsProvider` and `BasePathFsProvider`.**

`RootFsProvider` is the simplest case: you pass a single filesystem implementation to the constructor, and the provider will return that very same filesystem, whichever the path you request is.

`BasePathFsProvider` maps different filesystems to different folders relative to the root filesystem, e.g.:

    /first => ThisFilesystemImpl
    /second => ThatFilesystemImpl

In this scenario, the returned path will be the one you passed to FromPath, minus the first path component, e.g. if

    CreateDirectory("/second/hello/world")

is being called, the request will be forwarded as:

    ThatFilesystemImpl.CreateDirectory("/hello/world")

It is possible, although discouraged, to provide your own implementation of `IVsRemoteFileSystemProvider`. The general contract of this interface is: whichever the passed path is, you should always return a filesystem.

The only scenario in which you are required to supply your own provider implementation is that in which you need to differentiate the filesystem behavior based on the requesting user; more about this in the next chapter.



## Authentication and permission handling

Vs.Remote supports both key-based and username/password authentication, but there is no predefined authentication backend provided: you have to implement your own.

Vs.Remote session handling is token based: the authentication method must return a session token that is verified before any filesystem action.

In order to create an authentication backend, you should extend the `VsRemoteBaseAuthenticator` abstract class and override either `Authenticate(string auth_key)` or `Authenticate(string username, string password)`; these methods return a `VsRemoteAuthenticateResult` object that can be instantiated using static constructors present on the class itself, that are:

    VsRemoteAuthenticateResult.Authenticated(session_token)
    VsRemoteAuthenticateResult.InvalidAuthKey
    VsRemoteAuthenticateResult.InvalidUsernameOrPassword
    VsRemoteAuthenticateResult.AuthenticationError(errorMessage) // errorMessage is optional

Then you have to implement the `ValidateToken(string auth_token)` method that returns an `enum VsRemoteAuthenticationStatus`

 - valid token => return `AUTHENTICATED`
 - invalid token (for whatever reason) => return `EXPIRED`
 - authentication backend unavailable or faulty => return `AUTHENTICATION_ERROR`

**Other enum values should not be used as return values for `ValidateToken`.**

Whenever a token expires (i.e., you return `EXPIRED` from `ValidateToken`) the vscode extension automatically calls the Login method again in order to obtain a new token without any kind of user feedback inside the editor.

The Sample project contains an example of a token-based authentication backend with expiring session tokens inside the `SampleAuthKeyAuthentication.cs` file.

Now, if you need to differentiate the filesystem permissions based on the session, you have to dig a bit more. The entry point for implementing this feature is the `IVsRemoteFileSystemProvider` interface.

As previously stated, the `FromPath` method takes the `auth_token` as second parameter; in the provided implementations, this parameter is ignored.

If, for example, you want to provide a read-only and a read-write version of the very same filesystem, based on the requesting user, you might check the `auth_token`, verify if that token is owned by a user that has read-only or read-write permissions, and return the filesystem object accordingly.



## Initialize the server

In order to initialize the server-side of Vs.Remote you just have to add the Vs.Remote service via the `IServiceCollection.AddVsRemote( )` extension method, then map the service to the host via the `IEndpointRouteBuilder.MapVsRemote( )` extension method.

The simplest Main method possible is:

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        IVsRemoteFileSystemProvider fsProvider =
	        new RootFsProvider(
		        new InMemoryIndexedDictionaryFilesystem());
        builder.Services.AddVsRemote(fsProvider);
        var app = builder.Build();
        app.MapVsRemote();
        app.Run();
    }

The `AddVsRemote` method accepts a mandatory `IVsRemoteFileSystem` provider, along with an optional `IVsRemoteAuthenticator` to provide authentication. Lastly, you can provide an action based on `Grpc.AspNetCore.Server.GrpcServiceOptions` that will be forwarded to the underlying `AddGrpc( )` call.

The default for this object are:

    MaxReceiveMessageSize = int.MaxValue;
    MaxSendMessageSize = int.MaxValue;

If you need more info on these parameters and the other parameters available, please refer to the  [official documentation](https://grpc.github.io/grpc/csharp-dotnet/api/Grpc.AspNetCore.Server.GrpcServiceOptions.html).

The mapping function `MapVsRemote` receives an optional action based on the `VsRemoteOptions` that provides only these two options:

    public string RootFsPath { get; set; } = "/";
    public bool EnableReflectionService { get; set; } = false;

The first one allows you to set the gRPC service on a path other than the default root path ( "/" ); this requires you to have a reverse proxy or other similar facility, because the currently adopted client library does not allow you to invoke gRPC services on a different path.

The `EnableReflectionService` just enables the gRPC reflection service for debugging purposes; if you need any more detail, please refer to the `IEndpointRouteBuilder.MapGrpcReflectionService( )` extension method [official documentation](https://grpc.github.io/grpc/csharp-dotnet/api/Microsoft.AspNetCore.Builder.GrpcReflectionEndpointRouteBuilderExtensions.html).



## Configure the Visual Studio Code extension

After having installed the vscode Vs.Remote extension, in order to configure the available connections you just have to add a "vs.remote" json block inside your `settings.json` file following this sample schema:

    "vs.remote": {
      "remotes": {
        "test_conn_1": {
          "host": "localhost",
          "port": 5229
        },
        "test_conn_2": {
          "host": "localhost",
          "port": 5229,
          "auth_key": "1234567890"
        },
        "test_conn_3": {
          "host": "localhost",
          "port": 5229,
          "username": "guest"
        }
      }
    }

Here test_conn_1 is configured as anonymous / unauthenticated, test_conn_2 uses an authentication key, and test_conn_3 adopts the username/password auth scheme; you can configure the "password" field inside the configuration file, otherwise the password will be asked by vscode as soon as you try to connect to that endpoint. **Passwords are never stored anywhere automatically**.



## Sizes and limits

If you want to enforce a file size limit on the filesystem, you shouldn't do it inside your filesystem implementation, instead, you should better work on the system configuration following these steps:

 - Add the `GrpcServiceOptions` flags:

        builder.Services.AddVsRemote(fsProvider, options =>
        {
            options.MaxReceiveMessageSize = <int limit in bytes>
            options.MaxSendMessageSize = <int limit in bytes>
        });

 - Unlock the Kestrel file max request body size:

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Limits.MaxRequestBodySize = <int limit in bytes>
        });

And that's it.



## Sample Project

The sample project contains various elements that will surely help you implementing your own filesystem extension; there you'll find two filesystem implementation stubs:

 - `InMemoryIndexedDictionaryFilesystem` is a very basic, key-based, in-memory filesystem, backed by a `ConcurrentDictionary`
 - `LocalFolderFilesystem` is a path-based filesystem mapper for a local directory

along with `SampleAuthKeyAuthentication`: a simple authentication mechanism that implements auth_key authentication and expiring tokens.

A sample usage of these classes can be found inside the `Program.cs` source file.
