// Original file: ../srv/Vs.Remote/Vs.Remote.gRPC/Protos/fs.proto

import type { Long } from '@grpc/proto-loader';

export interface ReadFileResponse {
  'length'?: (number | string | Long);
  'content'?: (Buffer | Uint8Array | string);
}

export interface ReadFileResponse__Output {
  'length': (number);
  'content': (Buffer);
}
