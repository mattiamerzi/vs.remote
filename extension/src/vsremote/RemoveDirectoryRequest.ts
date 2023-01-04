// Original file: ../srv/Vs.Remote/Vs.Remote.gRPC/Protos/fs.proto


export interface RemoveDirectoryRequest {
  'authToken'?: (string);
  'path'?: (string);
  'recursive'?: (boolean);
}

export interface RemoveDirectoryRequest__Output {
  'authToken': (string);
  'path': (string);
  'recursive': (boolean);
}
