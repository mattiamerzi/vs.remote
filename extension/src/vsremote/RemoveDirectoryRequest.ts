// Original file: ../backend/VsRemote/Protos/fs.proto


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
