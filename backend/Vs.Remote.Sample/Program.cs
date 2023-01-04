using Vs.Remote.Interfaces;
using Vs.Remote.Providers;
using Vs.Remote.Sample;
using Vs.Remote.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = null;
});

IVsRemoteFileSystemProvider fsProvider = new RootFsProvider(new InMemoryIndexedDictionaryFilesystem());
//IVsRemoteFileSystemProvider fsProvider = new RootFsProvider(new LocalFolderFilesystem(@"C:\temp\"));
/*
IVsRemoteFileSystemProvider fsProvider = new BasePathFsProvider(new()
        {
            { "idx", new InMemoryIndexedDictionaryFilesystem() },
            { "temp", new LocalFolderFilesystem(@"C:\temp\") }
        });
*/

//        builder.Services.AddVsRemote(fsProvider, options =>
builder.Services.AddVsRemote(fsProvider, new SampleAuthKeyAuthentication(), options =>
{
    options.EnableDetailedErrors = true;
});

var app = builder.Build();

app.MapVsRemote(options =>
{
    options.EnableReflectionService = true;
});

app.Run();
