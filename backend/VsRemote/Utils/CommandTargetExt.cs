using VsRemote.Enums;

namespace VsRemote.Utils;

public static class CommandTargetExt
{
    public static CommandTarget ToProtoBuf(this VsRemoteCommandTarget vsRemoteCommandTarget)
    => vsRemoteCommandTarget switch
    {
        VsRemoteCommandTarget.NONE => CommandTarget.NoTarget,
        VsRemoteCommandTarget.FILE => CommandTarget.FileTarget,
        _ => CommandTarget.NoTarget
    };

}
