// Original file: ../backend/VsRemote/Protos/fs.proto

import type { AuthResult as _vsremote_AuthResult } from '../vsremote/AuthResult';

export interface LoginResponse {
  'authResult'?: (_vsremote_AuthResult | keyof typeof _vsremote_AuthResult);
  'authToken'?: (string);
  'failureMessage'?: (string);
}

export interface LoginResponse__Output {
  'authResult': (_vsremote_AuthResult);
  'authToken': (string);
  'failureMessage': (string);
}
