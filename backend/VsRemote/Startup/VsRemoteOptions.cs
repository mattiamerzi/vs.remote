namespace VsRemote.Startup;

public class VsRemoteOptions
{
    internal VsRemoteOptions() { }

    public string RootFsPath { get; set; } = "/";
    public bool EnableReflectionService { get; set; } = false;

}
