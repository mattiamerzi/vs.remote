using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VsRemote.Model;

namespace VsRemote.Interfaces;

internal interface IVsRemoteCommands
{
    public IEnumerable<IVsRemoteCommand> GetCommands();
    public bool TryGetCommand(string name, [MaybeNullWhen(false)] out IVsRemoteCommand command);

}
