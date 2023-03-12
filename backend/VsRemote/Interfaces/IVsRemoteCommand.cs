using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VsRemote.Model;

namespace VsRemote.Interfaces;

public interface IVsRemoteCommand
{
    public string Name { get; }
    public string Description { get; }
    public IEnumerable<VsRemoteCommandParameter> Parameters { get; }
    public Task<VsRemoteCommandResult> Action(string auth_token, IVsRemoteFileSystem remoteFs, string relativePath, Dictionary<string, string> parameters);
}
