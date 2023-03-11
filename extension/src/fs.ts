import type * as grpc from '@grpc/grpc-js';
import type { EnumTypeDefinition, MessageTypeDefinition } from '@grpc/proto-loader';

import type { VsRemoteClient as _vsremote_VsRemoteClient, VsRemoteDefinition as _vsremote_VsRemoteDefinition } from './vsremote/VsRemote';

type SubtypeConstructor<Constructor extends new (...args: any) => any, Subtype> = {
  new(...args: ConstructorParameters<Constructor>): Subtype;
};

export interface ProtoGrpcType {
  vsremote: {
    AuthResult: EnumTypeDefinition
    Command: MessageTypeDefinition
    CommandParameter: MessageTypeDefinition
    CreateDirectoryRequest: MessageTypeDefinition
    CreateDirectoryResponse: MessageTypeDefinition
    DeleteFileRequest: MessageTypeDefinition
    DeleteFileResponse: MessageTypeDefinition
    ExecuteCommandRequest: MessageTypeDefinition
    ExecuteCommandResponse: MessageTypeDefinition
    ExecutionCommandParameter: MessageTypeDefinition
    FileType: EnumTypeDefinition
    ListCommandsRequest: MessageTypeDefinition
    ListCommandsResponse: MessageTypeDefinition
    ListDirectoryRequest: MessageTypeDefinition
    ListDirectoryResponse: MessageTypeDefinition
    LoginRequest: MessageTypeDefinition
    LoginResponse: MessageTypeDefinition
    ReadFileRequest: MessageTypeDefinition
    ReadFileResponse: MessageTypeDefinition
    RemoveDirectoryRequest: MessageTypeDefinition
    RemoveDirectoryResponse: MessageTypeDefinition
    RenameFileRequest: MessageTypeDefinition
    RenameFileResponse: MessageTypeDefinition
    StatRequest: MessageTypeDefinition
    StatResponse: MessageTypeDefinition
    VsFsEntry: MessageTypeDefinition
    VsRemote: SubtypeConstructor<typeof grpc.Client, _vsremote_VsRemoteClient> & { service: _vsremote_VsRemoteDefinition }
    WriteFileRequest: MessageTypeDefinition
    WriteFileResponse: MessageTypeDefinition
  }
}

