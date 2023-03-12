using VsRemote.Interfaces;
using VsRemote.Model;
using VsRemote.Providers;
using VsRemote.Sample;
using VsRemote.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = null;
});

// sample in-memory filesystem
IVsRemoteFileSystemProvider fsProvider = new RootFsProvider(new InMemoryIndexedDictionaryFilesystem());

// sample local-folder filesystem
//IVsRemoteFileSystemProvider fsProvider = new RootFsProvider(new LocalFolderFilesystem(@"C:\temp\"));

// sample base-path filesystem that maps the "idx" and "temp" folders to two different filesystem implementations
/*
IVsRemoteFileSystemProvider fsProvider = new BasePathFsProvider(new()
        {
            { "idx", new InMemoryIndexedDictionaryFilesystem() },
            { "temp", new LocalFolderFilesystem(@"C:\temp\") }
        });
*/

/*
// simple, commandless VsRemote configuration
        builder.Services.AddVsRemote(fsProvider, options =>
        //builder.Services.AddVsRemote(fsProvider, new SampleAuthKeyAuthentication(), options =>
        {
            options.EnableDetailedErrors = true;
        });
*/

// full VsRemote configuration with builder
builder.Services.AddVsRemote(vs =>
{
    vs
        .SetRemoteFileSystemProvider(fsProvider)
        .SetGrpcServiceOptions(go =>
            go.EnableDetailedErrors = true)
        .AddCommand(CommandFactory.FromAction(() => Console.WriteLine("ExecuteCommand OK!"), "test command"))
        .AddCommand(new SampleCommand());
});

var app = builder.Build();

app.MapVsRemote(options =>
{
    options.EnableReflectionService = true;
});

app.Run();

class SampleCommand : BaseRemoteCommand
{
    private static readonly List<VsRemoteCommandParameter> parameters = new()
    {
        new( "param-one", "First sample parameter" ),
        new( "param-two", "Second sample parameter" )
    };
    public override IEnumerable<VsRemoteCommandParameter> Parameters => parameters;

    public SampleCommand() : base("sample-command", "Sample command with parameters") { }

    public override Task<VsRemoteCommandResult> RunCommandAsync(string auth_token, IVsRemoteFileSystem remoteFs, string relativePath, Dictionary<string, string> parameters)
    {
        Console.WriteLine("Sample command invoked; params:");
        foreach (var p in parameters)
        {
            Console.WriteLine($"  {p.Key} = {p.Value}");
        }
        return Task.FromResult(Success()); // or Failure() -- you can even pass an optional message to both functions
    }

}
