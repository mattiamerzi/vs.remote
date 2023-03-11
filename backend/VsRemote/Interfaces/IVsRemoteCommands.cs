using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VsRemote.Model;

namespace VsRemote.Interfaces;

internal interface IVsRemoteCommands
{
    public IEnumerable<IVsRemoteCommand> GetCommands();
    public VsRemoteCommandResult Execute(string command);
}
