// Original file: ../srv/Vs.Remote/Vs.Remote.gRPC/Protos/fs.proto

import type { VsFsEntry as _vsremote_VsFsEntry, VsFsEntry__Output as _vsremote_VsFsEntry__Output } from '../vsremote/VsFsEntry';

export interface StatResponse {
  'fileInfo'?: (_vsremote_VsFsEntry | null);
}

export interface StatResponse__Output {
  'fileInfo': (_vsremote_VsFsEntry__Output | null);
}
