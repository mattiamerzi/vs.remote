using VsRemote.Interfaces;

namespace VsRemote.Model;

public abstract class ParameterlessCommand : BaseRemoteCommand
{
    public override IEnumerable<VsRemoteCommandParameter> Parameters => Enumerable.Empty<VsRemoteCommandParameter>();

    protected ParameterlessCommand(string name, string description) : base(name, description) { }

}
