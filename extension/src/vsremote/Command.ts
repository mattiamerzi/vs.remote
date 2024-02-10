// Original file: ../backend/VsRemote/Protos/fs.proto

import type { CommandTarget as _vsremote_CommandTarget } from '../vsremote/CommandTarget';
import type { CommandParameter as _vsremote_CommandParameter, CommandParameter__Output as _vsremote_CommandParameter__Output } from '../vsremote/CommandParameter';

export interface Command {
  'name'?: (string);
  'description'?: (string);
  'commandTarget'?: (_vsremote_CommandTarget | keyof typeof _vsremote_CommandTarget);
  'modifiesFileContent'?: (boolean);
  'params'?: (_vsremote_CommandParameter)[];
}

export interface Command__Output {
  'name': (string);
  'description': (string);
  'commandTarget': (_vsremote_CommandTarget);
  'modifiesFileContent': (boolean);
  'params': (_vsremote_CommandParameter__Output)[];
}
