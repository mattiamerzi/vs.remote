// Original file: ../backend/VsRemote/Protos/fs.proto

import type { FileType as _vsremote_FileType } from '../vsremote/FileType';
import type { Long } from '@grpc/proto-loader';

export interface VsFsEntry {
  'name'?: (string);
  'fileType'?: (_vsremote_FileType | keyof typeof _vsremote_FileType);
  'mtime'?: (number | string | Long);
  'ctime'?: (number | string | Long);
  'size'?: (number | string | Long);
}

export interface VsFsEntry__Output {
  'name': (string);
  'fileType': (_vsremote_FileType);
  'mtime': (number);
  'ctime': (number);
  'size': (number);
}
