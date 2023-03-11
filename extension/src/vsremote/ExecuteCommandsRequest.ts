// Original file: ../backend/VsRemote/Protos/fs.proto

import type { ExecutionCommandParameter as _vsremote_ExecutionCommandParameter, ExecutionCommandParameter__Output as _vsremote_ExecutionCommandParameter__Output } from '../vsremote/ExecutionCommandParameter';

export interface ExecuteCommandsRequest {
  'authToken'?: (string);
  'command'?: (string);
  'params'?: (_vsremote_ExecutionCommandParameter)[];
}

export interface ExecuteCommandsRequest__Output {
  'authToken': (string);
  'command': (string);
  'params': (_vsremote_ExecutionCommandParameter__Output)[];
}
