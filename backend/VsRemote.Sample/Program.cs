using System.Text;
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
        new( "param-one", "Unverified sample parameter" ),
        new( "param-two", "Second non-empty sample parameter", VsRemoteCommandParameterValidation.NonEmpty ),
        new( "param-three", "Third, integer sample parameter", VsRemoteCommandParameterValidation.Integer )
    };
    public override IEnumerable<VsRemoteCommandParameter> Parameters => parameters;
    public override bool CanChangeFile => true;

    public SampleCommand() : base("sample-command", "Sample command with parameters") { }

    public override async Task<VsRemoteCommandResult> RunCommandAsync(string auth_token, IVsRemoteFileSystem remoteFs, string relativePath, Dictionary<string, string> parameters)
    {
        var ro_mem = await remoteFs.ReadFile(relativePath);
        StringBuilder sb = new StringBuilder(Encoding.ASCII.GetString(ro_mem.ToArray()));
        sb.AppendLine().AppendLine("Sample command invoked; params:");
        foreach (var p in parameters)
        {
            sb.AppendLine($"  {p.Key} = {p.Value}");
        }
        await remoteFs.WriteFile(relativePath,
            new ReadOnlyMemory<byte>(
                Encoding.ASCII.GetBytes(sb.ToString())
            ),
        true, true);
        return Success(); // or Failure() -- you can even pass an optional message to both functions
    }

}
