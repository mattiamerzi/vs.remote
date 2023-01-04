// Original file: ../srv/Vs.Remote/Vs.Remote.gRPC/Protos/fs.proto


export interface WriteFileRequest {
  'authToken'?: (string);
  'path'?: (string);
  'create'?: (boolean);
  'overwrite'?: (boolean);
  'content'?: (Buffer | Uint8Array | string);
}

export interface WriteFileRequest__Output {
  'authToken': (string);
  'path': (string);
  'create': (boolean);
  'overwrite': (boolean);
  'content': (Buffer);
}
