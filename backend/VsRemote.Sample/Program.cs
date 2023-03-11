using VsRemote.Interfaces;
using VsRemote.Providers;
using VsRemote.Sample;
using VsRemote.Startup;

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

        builder.Services.AddVsRemote(fsProvider, options =>
//builder.Services.AddVsRemote(fsProvider, new SampleAuthKeyAuthentication(), options =>
{
    options.EnableDetailedErrors = true;
});

var app = builder.Build();

app.MapVsRemote(options =>
{
    options.EnableReflectionService = true;
});

app.Run();
