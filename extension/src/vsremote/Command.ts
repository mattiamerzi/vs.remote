// Original file: ../backend/VsRemote/Protos/fs.proto

import type { CommandParameter as _vsremote_CommandParameter, CommandParameter__Output as _vsremote_CommandParameter__Output } from '../vsremote/CommandParameter';

export interface Command {
  'name'?: (string);
  'description'?: (string);
  'modifiesFileContent'?: (boolean);
  'params'?: (_vsremote_CommandParameter)[];
}

export interface Command__Output {
  'name': (string);
  'description': (string);
  'modifiesFileContent': (boolean);
  'params': (_vsremote_CommandParameter__Output)[];
}
