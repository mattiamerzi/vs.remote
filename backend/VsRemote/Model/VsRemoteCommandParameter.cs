using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VsRemote.Interfaces;

namespace VsRemote.Model;

public record VsRemoteCommandParameter (string Name, string Description, VsRemoteCommandParameterValidation Validation = VsRemoteCommandParameterValidation.None);

public enum VsRemoteCommandParameterValidation
{
    None,
    Integer,
    NonEmpty
}
