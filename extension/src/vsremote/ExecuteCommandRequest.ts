// Original file: ../backend/VsRemote/Protos/fs.proto

import type { ExecutionCommandParameter as _vsremote_ExecutionCommandParameter, ExecutionCommandParameter__Output as _vsremote_ExecutionCommandParameter__Output } from '../vsremote/ExecutionCommandParameter';

export interface ExecuteCommandRequest {
  'authToken'?: (string);
  'command'?: (string);
  'params'?: (_vsremote_ExecutionCommandParameter)[];
}

export interface ExecuteCommandRequest__Output {
  'authToken': (string);
  'command': (string);
  'params': (_vsremote_ExecutionCommandParameter__Output)[];
}
