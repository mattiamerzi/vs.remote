// Original file: ../backend/VsRemote/Protos/fs.proto

import type * as grpc from '@grpc/grpc-js'
import type { MethodDefinition } from '@grpc/proto-loader'
import type { CreateDirectoryRequest as _vsremote_CreateDirectoryRequest, CreateDirectoryRequest__Output as _vsremote_CreateDirectoryRequest__Output } from '../vsremote/CreateDirectoryRequest';
import type { CreateDirectoryResponse as _vsremote_CreateDirectoryResponse, CreateDirectoryResponse__Output as _vsremote_CreateDirectoryResponse__Output } from '../vsremote/CreateDirectoryResponse';
import type { DeleteFileRequest as _vsremote_DeleteFileRequest, DeleteFileRequest__Output as _vsremote_DeleteFileRequest__Output } from '../vsremote/DeleteFileRequest';
import type { DeleteFileResponse as _vsremote_DeleteFileResponse, DeleteFileResponse__Output as _vsremote_DeleteFileResponse__Output } from '../vsremote/DeleteFileResponse';
import type { ExecuteCommandRequest as _vsremote_ExecuteCommandRequest, ExecuteCommandRequest__Output as _vsremote_ExecuteCommandRequest__Output } from '../vsremote/ExecuteCommandRequest';
import type { ExecuteCommandResponse as _vsremote_ExecuteCommandResponse, ExecuteCommandResponse__Output as _vsremote_ExecuteCommandResponse__Output } from '../vsremote/ExecuteCommandResponse';
import type { ListCommandsRequest as _vsremote_ListCommandsRequest, ListCommandsRequest__Output as _vsremote_ListCommandsRequest__Output } from '../vsremote/ListCommandsRequest';
import type { ListCommandsResponse as _vsremote_ListCommandsResponse, ListCommandsResponse__Output as _vsremote_ListCommandsResponse__Output } from '../vsremote/ListCommandsResponse';
import type { ListDirectoryRequest as _vsremote_ListDirectoryRequest, ListDirectoryRequest__Output as _vsremote_ListDirectoryRequest__Output } from '../vsremote/ListDirectoryRequest';
import type { ListDirectoryResponse as _vsremote_ListDirectoryResponse, ListDirectoryResponse__Output as _vsremote_ListDirectoryResponse__Output } from '../vsremote/ListDirectoryResponse';
import type { LoginRequest as _vsremote_LoginRequest, LoginRequest__Output as _vsremote_LoginRequest__Output } from '../vsremote/LoginRequest';
import type { LoginResponse as _vsremote_LoginResponse, LoginResponse__Output as _vsremote_LoginResponse__Output } from '../vsremote/LoginResponse';
import type { ReadFileRequest as _vsremote_ReadFileRequest, ReadFileRequest__Output as _vsremote_ReadFileRequest__Output } from '../vsremote/ReadFileRequest';
import type { ReadFileResponse as _vsremote_ReadFileResponse, ReadFileResponse__Output as _vsremote_ReadFileResponse__Output } from '../vsremote/ReadFileResponse';
import type { RemoveDirectoryRequest as _vsremote_RemoveDirectoryRequest, RemoveDirectoryRequest__Output as _vsremote_RemoveDirectoryRequest__Output } from '../vsremote/RemoveDirectoryRequest';
import type { RemoveDirectoryResponse as _vsremote_RemoveDirectoryResponse, RemoveDirectoryResponse__Output as _vsremote_RemoveDirectoryResponse__Output } from '../vsremote/RemoveDirectoryResponse';
import type { RenameFileRequest as _vsremote_RenameFileRequest, RenameFileRequest__Output as _vsremote_RenameFileRequest__Output } from '../vsremote/RenameFileRequest';
import type { RenameFileResponse as _vsremote_RenameFileResponse, RenameFileResponse__Output as _vsremote_RenameFileResponse__Output } from '../vsremote/RenameFileResponse';
import type { StatRequest as _vsremote_StatRequest, StatRequest__Output as _vsremote_StatRequest__Output } from '../vsremote/StatRequest';
import type { StatResponse as _vsremote_StatResponse, StatResponse__Output as _vsremote_StatResponse__Output } from '../vsremote/StatResponse';
import type { WriteFileRequest as _vsremote_WriteFileRequest, WriteFileRequest__Output as _vsremote_WriteFileRequest__Output } from '../vsremote/WriteFileRequest';
import type { WriteFileResponse as _vsremote_WriteFileResponse, WriteFileResponse__Output as _vsremote_WriteFileResponse__Output } from '../vsremote/WriteFileResponse';

export interface VsRemoteClient extends grpc.Client {
  CreateDirectory(argument: _vsremote_CreateDirectoryRequest, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_CreateDirectoryResponse__Output>): grpc.ClientUnaryCall;
  CreateDirectory(argument: _vsremote_CreateDirectoryRequest, metadata: grpc.Metadata, callback: grpc.requestCallback<_vsremote_CreateDirectoryResponse__Output>): grpc.ClientUnaryCall;
  CreateDirectory(argument: _vsremote_CreateDirectoryRequest, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_CreateDirectoryResponse__Output>): grpc.ClientUnaryCall;
  CreateDirectory(argument: _vsremote_CreateDirectoryRequest, callback: grpc.requestCallback<_vsremote_CreateDirectoryResponse__Output>): grpc.ClientUnaryCall;
  createDirectory(argument: _vsremote_CreateDirectoryRequest, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_CreateDirectoryResponse__Output>): grpc.ClientUnaryCall;
  createDirectory(argument: _vsremote_CreateDirectoryRequest, metadata: grpc.Metadata, callback: grpc.requestCallback<_vsremote_CreateDirectoryResponse__Output>): grpc.ClientUnaryCall;
  createDirectory(argument: _vsremote_CreateDirectoryRequest, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_CreateDirectoryResponse__Output>): grpc.ClientUnaryCall;
  createDirectory(argument: _vsremote_CreateDirectoryRequest, callback: grpc.requestCallback<_vsremote_CreateDirectoryResponse__Output>): grpc.ClientUnaryCall;
  
  DeleteFile(argument: _vsremote_DeleteFileRequest, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_DeleteFileResponse__Output>): grpc.ClientUnaryCall;
  DeleteFile(argument: _vsremote_DeleteFileRequest, metadata: grpc.Metadata, callback: grpc.requestCallback<_vsremote_DeleteFileResponse__Output>): grpc.ClientUnaryCall;
  DeleteFile(argument: _vsremote_DeleteFileRequest, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_DeleteFileResponse__Output>): grpc.ClientUnaryCall;
  DeleteFile(argument: _vsremote_DeleteFileRequest, callback: grpc.requestCallback<_vsremote_DeleteFileResponse__Output>): grpc.ClientUnaryCall;
  deleteFile(argument: _vsremote_DeleteFileRequest, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_DeleteFileResponse__Output>): grpc.ClientUnaryCall;
  deleteFile(argument: _vsremote_DeleteFileRequest, metadata: grpc.Metadata, callback: grpc.requestCallback<_vsremote_DeleteFileResponse__Output>): grpc.ClientUnaryCall;
  deleteFile(argument: _vsremote_DeleteFileRequest, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_DeleteFileResponse__Output>): grpc.ClientUnaryCall;
  deleteFile(argument: _vsremote_DeleteFileRequest, callback: grpc.requestCallback<_vsremote_DeleteFileResponse__Output>): grpc.ClientUnaryCall;
  
  ExecuteCommand(argument: _vsremote_ExecuteCommandRequest, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_ExecuteCommandResponse__Output>): grpc.ClientUnaryCall;
  ExecuteCommand(argument: _vsremote_ExecuteCommandRequest, metadata: grpc.Metadata, callback: grpc.requestCallback<_vsremote_ExecuteCommandResponse__Output>): grpc.ClientUnaryCall;
  ExecuteCommand(argument: _vsremote_ExecuteCommandRequest, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_ExecuteCommandResponse__Output>): grpc.ClientUnaryCall;
  ExecuteCommand(argument: _vsremote_ExecuteCommandRequest, callback: grpc.requestCallback<_vsremote_ExecuteCommandResponse__Output>): grpc.ClientUnaryCall;
  executeCommand(argument: _vsremote_ExecuteCommandRequest, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_ExecuteCommandResponse__Output>): grpc.ClientUnaryCall;
  executeCommand(argument: _vsremote_ExecuteCommandRequest, metadata: grpc.Metadata, callback: grpc.requestCallback<_vsremote_ExecuteCommandResponse__Output>): grpc.ClientUnaryCall;
  executeCommand(argument: _vsremote_ExecuteCommandRequest, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_ExecuteCommandResponse__Output>): grpc.ClientUnaryCall;
  executeCommand(argument: _vsremote_ExecuteCommandRequest, callback: grpc.requestCallback<_vsremote_ExecuteCommandResponse__Output>): grpc.ClientUnaryCall;
  
  ListCommands(argument: _vsremote_ListCommandsRequest, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_ListCommandsResponse__Output>): grpc.ClientUnaryCall;
  ListCommands(argument: _vsremote_ListCommandsRequest, metadata: grpc.Metadata, callback: grpc.requestCallback<_vsremote_ListCommandsResponse__Output>): grpc.ClientUnaryCall;
  ListCommands(argument: _vsremote_ListCommandsRequest, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_ListCommandsResponse__Output>): grpc.ClientUnaryCall;
  ListCommands(argument: _vsremote_ListCommandsRequest, callback: grpc.requestCallback<_vsremote_ListCommandsResponse__Output>): grpc.ClientUnaryCall;
  listCommands(argument: _vsremote_ListCommandsRequest, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_ListCommandsResponse__Output>): grpc.ClientUnaryCall;
  listCommands(argument: _vsremote_ListCommandsRequest, metadata: grpc.Metadata, callback: grpc.requestCallback<_vsremote_ListCommandsResponse__Output>): grpc.ClientUnaryCall;
  listCommands(argument: _vsremote_ListCommandsRequest, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_ListCommandsResponse__Output>): grpc.ClientUnaryCall;
  listCommands(argument: _vsremote_ListCommandsRequest, callback: grpc.requestCallback<_vsremote_ListCommandsResponse__Output>): grpc.ClientUnaryCall;
  
  ListDirectory(argument: _vsremote_ListDirectoryRequest, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_ListDirectoryResponse__Output>): grpc.ClientUnaryCall;
  ListDirectory(argument: _vsremote_ListDirectoryRequest, metadata: grpc.Metadata, callback: grpc.requestCallback<_vsremote_ListDirectoryResponse__Output>): grpc.ClientUnaryCall;
  ListDirectory(argument: _vsremote_ListDirectoryRequest, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_ListDirectoryResponse__Output>): grpc.ClientUnaryCall;
  ListDirectory(argument: _vsremote_ListDirectoryRequest, callback: grpc.requestCallback<_vsremote_ListDirectoryResponse__Output>): grpc.ClientUnaryCall;
  listDirectory(argument: _vsremote_ListDirectoryRequest, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_ListDirectoryResponse__Output>): grpc.ClientUnaryCall;
  listDirectory(argument: _vsremote_ListDirectoryRequest, metadata: grpc.Metadata, callback: grpc.requestCallback<_vsremote_ListDirectoryResponse__Output>): grpc.ClientUnaryCall;
  listDirectory(argument: _vsremote_ListDirectoryRequest, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_ListDirectoryResponse__Output>): grpc.ClientUnaryCall;
  listDirectory(argument: _vsremote_ListDirectoryRequest, callback: grpc.requestCallback<_vsremote_ListDirectoryResponse__Output>): grpc.ClientUnaryCall;
  
  Login(argument: _vsremote_LoginRequest, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_LoginResponse__Output>): grpc.ClientUnaryCall;
  Login(argument: _vsremote_LoginRequest, metadata: grpc.Metadata, callback: grpc.requestCallback<_vsremote_LoginResponse__Output>): grpc.ClientUnaryCall;
  Login(argument: _vsremote_LoginRequest, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_LoginResponse__Output>): grpc.ClientUnaryCall;
  Login(argument: _vsremote_LoginRequest, callback: grpc.requestCallback<_vsremote_LoginResponse__Output>): grpc.ClientUnaryCall;
  login(argument: _vsremote_LoginRequest, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_LoginResponse__Output>): grpc.ClientUnaryCall;
  login(argument: _vsremote_LoginRequest, metadata: grpc.Metadata, callback: grpc.requestCallback<_vsremote_LoginResponse__Output>): grpc.ClientUnaryCall;
  login(argument: _vsremote_LoginRequest, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_LoginResponse__Output>): grpc.ClientUnaryCall;
  login(argument: _vsremote_LoginRequest, callback: grpc.requestCallback<_vsremote_LoginResponse__Output>): grpc.ClientUnaryCall;
  
  ReadFile(argument: _vsremote_ReadFileRequest, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_ReadFileResponse__Output>): grpc.ClientUnaryCall;
  ReadFile(argument: _vsremote_ReadFileRequest, metadata: grpc.Metadata, callback: grpc.requestCallback<_vsremote_ReadFileResponse__Output>): grpc.ClientUnaryCall;
  ReadFile(argument: _vsremote_ReadFileRequest, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_ReadFileResponse__Output>): grpc.ClientUnaryCall;
  ReadFile(argument: _vsremote_ReadFileRequest, callback: grpc.requestCallback<_vsremote_ReadFileResponse__Output>): grpc.ClientUnaryCall;
  readFile(argument: _vsremote_ReadFileRequest, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_ReadFileResponse__Output>): grpc.ClientUnaryCall;
  readFile(argument: _vsremote_ReadFileRequest, metadata: grpc.Metadata, callback: grpc.requestCallback<_vsremote_ReadFileResponse__Output>): grpc.ClientUnaryCall;
  readFile(argument: _vsremote_ReadFileRequest, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_ReadFileResponse__Output>): grpc.ClientUnaryCall;
  readFile(argument: _vsremote_ReadFileRequest, callback: grpc.requestCallback<_vsremote_ReadFileResponse__Output>): grpc.ClientUnaryCall;
  
  RemoveDirectory(argument: _vsremote_RemoveDirectoryRequest, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_RemoveDirectoryResponse__Output>): grpc.ClientUnaryCall;
  RemoveDirectory(argument: _vsremote_RemoveDirectoryRequest, metadata: grpc.Metadata, callback: grpc.requestCallback<_vsremote_RemoveDirectoryResponse__Output>): grpc.ClientUnaryCall;
  RemoveDirectory(argument: _vsremote_RemoveDirectoryRequest, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_RemoveDirectoryResponse__Output>): grpc.ClientUnaryCall;
  RemoveDirectory(argument: _vsremote_RemoveDirectoryRequest, callback: grpc.requestCallback<_vsremote_RemoveDirectoryResponse__Output>): grpc.ClientUnaryCall;
  removeDirectory(argument: _vsremote_RemoveDirectoryRequest, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_RemoveDirectoryResponse__Output>): grpc.ClientUnaryCall;
  removeDirectory(argument: _vsremote_RemoveDirectoryRequest, metadata: grpc.Metadata, callback: grpc.requestCallback<_vsremote_RemoveDirectoryResponse__Output>): grpc.ClientUnaryCall;
  removeDirectory(argument: _vsremote_RemoveDirectoryRequest, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_RemoveDirectoryResponse__Output>): grpc.ClientUnaryCall;
  removeDirectory(argument: _vsremote_RemoveDirectoryRequest, callback: grpc.requestCallback<_vsremote_RemoveDirectoryResponse__Output>): grpc.ClientUnaryCall;
  
  RenameFile(argument: _vsremote_RenameFileRequest, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_RenameFileResponse__Output>): grpc.ClientUnaryCall;
  RenameFile(argument: _vsremote_RenameFileRequest, metadata: grpc.Metadata, callback: grpc.requestCallback<_vsremote_RenameFileResponse__Output>): grpc.ClientUnaryCall;
  RenameFile(argument: _vsremote_RenameFileRequest, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_RenameFileResponse__Output>): grpc.ClientUnaryCall;
  RenameFile(argument: _vsremote_RenameFileRequest, callback: grpc.requestCallback<_vsremote_RenameFileResponse__Output>): grpc.ClientUnaryCall;
  renameFile(argument: _vsremote_RenameFileRequest, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_RenameFileResponse__Output>): grpc.ClientUnaryCall;
  renameFile(argument: _vsremote_RenameFileRequest, metadata: grpc.Metadata, callback: grpc.requestCallback<_vsremote_RenameFileResponse__Output>): grpc.ClientUnaryCall;
  renameFile(argument: _vsremote_RenameFileRequest, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_RenameFileResponse__Output>): grpc.ClientUnaryCall;
  renameFile(argument: _vsremote_RenameFileRequest, callback: grpc.requestCallback<_vsremote_RenameFileResponse__Output>): grpc.ClientUnaryCall;
  
  Stat(argument: _vsremote_StatRequest, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_StatResponse__Output>): grpc.ClientUnaryCall;
  Stat(argument: _vsremote_StatRequest, metadata: grpc.Metadata, callback: grpc.requestCallback<_vsremote_StatResponse__Output>): grpc.ClientUnaryCall;
  Stat(argument: _vsremote_StatRequest, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_StatResponse__Output>): grpc.ClientUnaryCall;
  Stat(argument: _vsremote_StatRequest, callback: grpc.requestCallback<_vsremote_StatResponse__Output>): grpc.ClientUnaryCall;
  stat(argument: _vsremote_StatRequest, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_StatResponse__Output>): grpc.ClientUnaryCall;
  stat(argument: _vsremote_StatRequest, metadata: grpc.Metadata, callback: grpc.requestCallback<_vsremote_StatResponse__Output>): grpc.ClientUnaryCall;
  stat(argument: _vsremote_StatRequest, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_StatResponse__Output>): grpc.ClientUnaryCall;
  stat(argument: _vsremote_StatRequest, callback: grpc.requestCallback<_vsremote_StatResponse__Output>): grpc.ClientUnaryCall;
  
  WriteFile(argument: _vsremote_WriteFileRequest, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_WriteFileResponse__Output>): grpc.ClientUnaryCall;
  WriteFile(argument: _vsremote_WriteFileRequest, metadata: grpc.Metadata, callback: grpc.requestCallback<_vsremote_WriteFileResponse__Output>): grpc.ClientUnaryCall;
  WriteFile(argument: _vsremote_WriteFileRequest, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_WriteFileResponse__Output>): grpc.ClientUnaryCall;
  WriteFile(argument: _vsremote_WriteFileRequest, callback: grpc.requestCallback<_vsremote_WriteFileResponse__Output>): grpc.ClientUnaryCall;
  writeFile(argument: _vsremote_WriteFileRequest, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_WriteFileResponse__Output>): grpc.ClientUnaryCall;
  writeFile(argument: _vsremote_WriteFileRequest, metadata: grpc.Metadata, callback: grpc.requestCallback<_vsremote_WriteFileResponse__Output>): grpc.ClientUnaryCall;
  writeFile(argument: _vsremote_WriteFileRequest, options: grpc.CallOptions, callback: grpc.requestCallback<_vsremote_WriteFileResponse__Output>): grpc.ClientUnaryCall;
  writeFile(argument: _vsremote_WriteFileRequest, callback: grpc.requestCallback<_vsremote_WriteFileResponse__Output>): grpc.ClientUnaryCall;
  
}

export interface VsRemoteHandlers extends grpc.UntypedServiceImplementation {
  CreateDirectory: grpc.handleUnaryCall<_vsremote_CreateDirectoryRequest__Output, _vsremote_CreateDirectoryResponse>;
  
  DeleteFile: grpc.handleUnaryCall<_vsremote_DeleteFileRequest__Output, _vsremote_DeleteFileResponse>;
  
  ExecuteCommand: grpc.handleUnaryCall<_vsremote_ExecuteCommandRequest__Output, _vsremote_ExecuteCommandResponse>;
  
  ListCommands: grpc.handleUnaryCall<_vsremote_ListCommandsRequest__Output, _vsremote_ListCommandsResponse>;
  
  ListDirectory: grpc.handleUnaryCall<_vsremote_ListDirectoryRequest__Output, _vsremote_ListDirectoryResponse>;
  
  Login: grpc.handleUnaryCall<_vsremote_LoginRequest__Output, _vsremote_LoginResponse>;
  
  ReadFile: grpc.handleUnaryCall<_vsremote_ReadFileRequest__Output, _vsremote_ReadFileResponse>;
  
  RemoveDirectory: grpc.handleUnaryCall<_vsremote_RemoveDirectoryRequest__Output, _vsremote_RemoveDirectoryResponse>;
  
  RenameFile: grpc.handleUnaryCall<_vsremote_RenameFileRequest__Output, _vsremote_RenameFileResponse>;
  
  Stat: grpc.handleUnaryCall<_vsremote_StatRequest__Output, _vsremote_StatResponse>;
  
  WriteFile: grpc.handleUnaryCall<_vsremote_WriteFileRequest__Output, _vsremote_WriteFileResponse>;
  
}

export interface VsRemoteDefinition extends grpc.ServiceDefinition {
  CreateDirectory: MethodDefinition<_vsremote_CreateDirectoryRequest, _vsremote_CreateDirectoryResponse, _vsremote_CreateDirectoryRequest__Output, _vsremote_CreateDirectoryResponse__Output>
  DeleteFile: MethodDefinition<_vsremote_DeleteFileRequest, _vsremote_DeleteFileResponse, _vsremote_DeleteFileRequest__Output, _vsremote_DeleteFileResponse__Output>
  ExecuteCommand: MethodDefinition<_vsremote_ExecuteCommandRequest, _vsremote_ExecuteCommandResponse, _vsremote_ExecuteCommandRequest__Output, _vsremote_ExecuteCommandResponse__Output>
  ListCommands: MethodDefinition<_vsremote_ListCommandsRequest, _vsremote_ListCommandsResponse, _vsremote_ListCommandsRequest__Output, _vsremote_ListCommandsResponse__Output>
  ListDirectory: MethodDefinition<_vsremote_ListDirectoryRequest, _vsremote_ListDirectoryResponse, _vsremote_ListDirectoryRequest__Output, _vsremote_ListDirectoryResponse__Output>
  Login: MethodDefinition<_vsremote_LoginRequest, _vsremote_LoginResponse, _vsremote_LoginRequest__Output, _vsremote_LoginResponse__Output>
  ReadFile: MethodDefinition<_vsremote_ReadFileRequest, _vsremote_ReadFileResponse, _vsremote_ReadFileRequest__Output, _vsremote_ReadFileResponse__Output>
  RemoveDirectory: MethodDefinition<_vsremote_RemoveDirectoryRequest, _vsremote_RemoveDirectoryResponse, _vsremote_RemoveDirectoryRequest__Output, _vsremote_RemoveDirectoryResponse__Output>
  RenameFile: MethodDefinition<_vsremote_RenameFileRequest, _vsremote_RenameFileResponse, _vsremote_RenameFileRequest__Output, _vsremote_RenameFileResponse__Output>
  Stat: MethodDefinition<_vsremote_StatRequest, _vsremote_StatResponse, _vsremote_StatRequest__Output, _vsremote_StatResponse__Output>
  WriteFile: MethodDefinition<_vsremote_WriteFileRequest, _vsremote_WriteFileResponse, _vsremote_WriteFileRequest__Output, _vsremote_WriteFileResponse__Output>
}
