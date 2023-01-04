namespace VsRemote.Enums;

// from Visual Studio Code source
public enum VsRemoteFileType
{
    Unknown = 0,
    /**
     * A regular file.
     */
    File = 1,
    /**
     * A directory.
     */
    Directory = 2,
    /**
     * A symbolic link to a file.
     */
    SymbolicLink = 64

}
