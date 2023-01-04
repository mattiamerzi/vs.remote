import * as vscode from 'vscode';
import * as grpc from '@grpc/grpc-js';

function metadataToString(metadata: grpc.Metadata, key: string, defaultValue: string) {
    if (!metadata)
        return defaultValue;

    const mdArray: grpc.MetadataValue[] = metadata.get(key);

    if (mdArray && mdArray.length > 0)
        return mdArray[0] as string;
    else
        return defaultValue;
}

export function tokenExpired(err: grpc.ServiceError): boolean {
    return metadataToString(err.metadata, 'error_code', 'E_UNKNOWN') == 'EXPIRED';
}

export function reportError(err: grpc.ServiceError, uri: vscode.Uri) {
    const errorCode: string = metadataToString(err.metadata, 'error_code', 'E_UNKNOWN');
    const errorMessage: string = metadataToString(err.metadata, 'error_message', err.details || 'Unknown Error');
    let exception: vscode.FileSystemError;
    switch (errorCode) {
        case 'E_PERM_DENIED': exception = vscode.FileSystemError.NoPermissions(uri); break;
        case 'E_NOT_FOUND': exception = vscode.FileSystemError.FileNotFound(uri); break;
        case 'E_NOT_DIR': exception = vscode.FileSystemError.FileNotADirectory(uri); break;
        case 'E_IS_DIR': exception = vscode.FileSystemError.FileIsADirectory(uri); break;
        case 'E_ERROR':
        case 'E_IO_ERROR': exception = vscode.FileSystemError.Unavailable(uri); break;
        case 'E_NOT_EMPTY': 
        case 'E_INVALID_PATH':
        case 'E_EXISTS':
        case 'E_UNKNOWN': exception = new vscode.FileSystemError(errorMessage); break;
        case 'INVALID_AUTH_KEY':
        case 'INVALID_USERNAME_OR_PASSWORD':
        case 'AUTHENTICATION_ERROR':
        case 'EXPIRED':
            exception = vscode.FileSystemError.NoPermissions(uri);
            break;
        default: exception = new vscode.FileSystemError(errorMessage);
    }
    vscode.window.showErrorMessage(errorMessage || errorCode);
    return exception;
}


