// Original file: ../backend/VsRemote/Protos/fs.proto

import type { Long } from '@grpc/proto-loader';

export interface ReadFileResponse {
  'length'?: (number | string | Long);
  'content'?: (Buffer | Uint8Array | string);
}

export interface ReadFileResponse__Output {
  'length': (number);
  'content': (Buffer);
}
