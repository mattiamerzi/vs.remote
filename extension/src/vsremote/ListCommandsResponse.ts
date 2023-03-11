// Original file: ../backend/VsRemote/Protos/fs.proto

import type { Command as _vsremote_Command, Command__Output as _vsremote_Command__Output } from '../vsremote/Command';

export interface ListCommandsResponse {
  'hasCommands'?: (boolean);
  'commands'?: (_vsremote_Command)[];
}

export interface ListCommandsResponse__Output {
  'hasCommands': (boolean);
  'commands': (_vsremote_Command__Output)[];
}
