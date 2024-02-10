// Original file: ../backend/VsRemote/Protos/fs.proto

import type { Long } from '@grpc/proto-loader';

export interface WriteFileResponse {
  'bytesWritten'?: (number | string | Long);
}

export interface WriteFileResponse__Output {
  'bytesWritten': (number);
}
