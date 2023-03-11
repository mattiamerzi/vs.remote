// Original file: ../backend/VsRemote/Protos/fs.proto

import type { VsFsEntry as _vsremote_VsFsEntry, VsFsEntry__Output as _vsremote_VsFsEntry__Output } from '../vsremote/VsFsEntry';

export interface ListDirectoryResponse {
  'entries'?: (_vsremote_VsFsEntry)[];
}

export interface ListDirectoryResponse__Output {
  'entries': (_vsremote_VsFsEntry__Output)[];
}
