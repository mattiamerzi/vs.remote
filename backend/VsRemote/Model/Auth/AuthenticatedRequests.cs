using VsRemote.Interfaces;

namespace VsRemote;

public partial class CreateDirectoryRequest: IAuthenticatedRequest { }
public partial class DeleteFileRequest: IAuthenticatedRequest { }
public partial class CreateFileRequest: IAuthenticatedRequest { }
public partial class WriteFileRequest: IAuthenticatedRequest { }
public partial class ReadFileRequest: IAuthenticatedRequest { }
public partial class ReadFileOffsetRequest: IAuthenticatedRequest { }
public partial class ListDirectoryRequest: IAuthenticatedRequest { }
public partial class ExecuteCommandRequest: IAuthenticatedRequest { }
public partial class ListCommandsRequest: IAuthenticatedRequest { }
public partial class RemoveDirectoryRequest: IAuthenticatedRequest { }
public partial class RenameFileRequest: IAuthenticatedRequest { }
public partial class StatRequest: IAuthenticatedRequest { }
public partial class WriteFileAppendRequest: IAuthenticatedRequest { }
public partial class WriteFileOffsetRequest: IAuthenticatedRequest { }
