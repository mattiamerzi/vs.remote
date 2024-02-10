// Original file: ../backend/VsRemote/Protos/fs.proto

import type { CommandParameterValidation as _vsremote_CommandParameterValidation } from '../vsremote/CommandParameterValidation';

export interface CommandParameter {
  'name'?: (string);
  'description'?: (string);
  'validation'?: (_vsremote_CommandParameterValidation | keyof typeof _vsremote_CommandParameterValidation);
}

export interface CommandParameter__Output {
  'name': (string);
  'description': (string);
  'validation': (_vsremote_CommandParameterValidation);
}
