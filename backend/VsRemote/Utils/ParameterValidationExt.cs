using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VsRemote.Model;

namespace VsRemote.Utils;

internal static class ParameterValidationExt
{
    public static CommandParameterValidation ToProtoBuf(this VsRemoteCommandParameterValidation validation)
        => validation switch
            {
                VsRemoteCommandParameterValidation.None => CommandParameterValidation.None,
                VsRemoteCommandParameterValidation.Integer => CommandParameterValidation.Integer,
                VsRemoteCommandParameterValidation.NonEmpty => CommandParameterValidation.NonEmpty,
                _ => CommandParameterValidation.None
            };
}
