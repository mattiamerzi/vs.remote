// Original file: ../backend/VsRemote/Protos/fs.proto


export interface RenameFileRequest {
  'authToken'?: (string);
  'fromPath'?: (string);
  'toPath'?: (string);
  'overwrite'?: (boolean);
}

export interface RenameFileRequest__Output {
  'authToken': (string);
  'fromPath': (string);
  'toPath': (string);
  'overwrite': (boolean);
}
