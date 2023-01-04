// Original file: ../srv/Vs.Remote/Vs.Remote.gRPC/Protos/fs.proto

import type { Long } from '@grpc/proto-loader';

export interface WriteFileResponse {
  'bytesWritten'?: (number | string | Long);
}

export interface WriteFileResponse__Output {
  'bytesWritten': (number);
}
