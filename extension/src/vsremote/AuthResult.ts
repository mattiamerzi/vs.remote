// Original file: ../backend/VsRemote/Protos/fs.proto

export enum AuthResult {
  AUTHENTICATED = 0,
  INVALID_AUTH_KEY = 1,
  INVALID_USERNAME_OR_PASSWORD = 2,
  AUTHENTICATION_ERROR = 3,
  EXPIRED = 4,
}
