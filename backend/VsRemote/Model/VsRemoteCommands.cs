using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VsRemote.Interfaces;

namespace VsRemote.Model;

internal class VsRemoteCommands : IVsRemoteCommands
{
    private readonly Dictionary<string, IVsRemoteCommand> commands = new();

    public IEnumerable<IVsRemoteCommand> GetCommands()
        => commands.Values;

    public void AddCommand(IVsRemoteCommand command)
        => commands.TryAdd(command.Name, command);

    public bool TryGetCommand(string name, [MaybeNullWhen(false)] out IVsRemoteCommand command)
        => commands.TryGetValue(name, out command);

}
