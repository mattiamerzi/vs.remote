import type { CreateDirectoryRequest } from './vsremote/CreateDirectoryRequest';
import type { CreateDirectoryResponse__Output } from './vsremote/CreateDirectoryResponse';
import type { DeleteFileRequest } from './vsremote/DeleteFileRequest';
import type { DeleteFileResponse__Output } from './vsremote/DeleteFileResponse';
import type { RemoveDirectoryRequest } from './vsremote/RemoveDirectoryRequest';
import type { RemoveDirectoryResponse__Output } from './vsremote/RemoveDirectoryResponse';
import type { ListDirectoryRequest } from './vsremote/ListDirectoryRequest';
import type { ListDirectoryResponse__Output } from './vsremote/ListDirectoryResponse';
import type { ReadFileRequest } from './vsremote/ReadFileRequest';
import type { ReadFileResponse__Output } from './vsremote/ReadFileResponse';
import type { RenameFileRequest } from './vsremote/RenameFileRequest';
import type { RenameFileResponse__Output } from './vsremote/RenameFileResponse';
import type { StatRequest } from './vsremote/StatRequest';
import type { StatResponse__Output } from './vsremote/StatResponse';
import type { LoginRequest } from './vsremote/LoginRequest';
import type { LoginResponse__Output } from './vsremote/LoginResponse';
import type { WriteFileRequest } from './vsremote/WriteFileRequest';
import type { WriteFileResponse__Output } from './vsremote/WriteFileResponse';
import { VsFsEntry__Output } from './vsremote/VsFsEntry';
import { FileType } from './vsremote/FileType';
import { VsRemoteClient } from './vsremote/VsRemote';

import { reportError, tokenExpired } from './reportError';
import * as path from 'path';
import * as vscode from 'vscode';
import * as grpc from '@grpc/grpc-js';
import init_gRPC from './initVsRemote';
import { VsRemoteHost } from './settings';
import { AuthResult } from './vsremote/AuthResult';

export class FileStat implements vscode.FileStat {
	type: vscode.FileType;
	ctime: number;
	mtime: number;
	size: number;

	constructor(type: vscode.FileType, ctime: number, mtime: number, size: number) {
		this.type = type;
		this.ctime = ctime || Date.now();
		this.mtime = mtime || Date.now();
		this.size = size || 0;
	}

	static fromVsFsEntry(vsFsEntry: VsFsEntry__Output) {
		const ftype:vscode.FileType = fsEntryTypeToVsCodeType(vsFsEntry.fileType);
		return new FileStat(ftype, vsFsEntry.ctime, vsFsEntry.mtime, vsFsEntry.size);
	}
}

function fsEntryTypeToVsCodeType(fileType: FileType):vscode.FileType  {
	return fileType as number as vscode.FileType;
}

export class VsRemoteFsProvider implements vscode.FileSystemProvider {
	client: VsRemoteClient | undefined;
	remote: VsRemoteHost | undefined;
	connectHost: string = '';
	auth_token: string = '';

	constructor(remote: VsRemoteHost) {
		const connectHost = `${remote.host}:${remote.port}`;
		console.log(`Vs.Remote connect(${connectHost})`);
		const client: VsRemoteClient = init_gRPC(connectHost);
		this.remote = remote;
		this.client = client;
	}

	login(): Promise<boolean> {
		if (this.remote == undefined)
			return Promise.resolve(false);
		const connectHost = this.connectHost;
		const remote = this.remote;
		let loginRequest: LoginRequest | undefined;
		switch (remote.auth_method) {
			case 'auth_key':
				loginRequest = {
					authKey: remote.auth_key
				};
				break;
			case 'password':
				loginRequest = {
					username: remote.username,
					password: remote.password
				};
				break;
			case 'none': return Promise.resolve(true);
		}
		if (loginRequest == undefined) {
			return Promise.resolve(true);
		} else {
			const dloginRequest = loginRequest;
			return new Promise<boolean>((resolve, reject) => {
				if (this.client == undefined) { return reject(vscode.FileSystemError.Unavailable(connectHost)); }
				this.client.Login(dloginRequest, (err: grpc.ServiceError | null, feature: LoginResponse__Output | undefined) => {
					if (!err && feature) {
						console.log(`OK : Login(${remote.auth_key} ~ ${remote.username}) = ${JSON.stringify(feature)}`);
						if (feature.authResult == AuthResult.AUTHENTICATED) {
							this.auth_token = feature.authToken;
							resolve(true);
						} else {
							vscode.window.showErrorMessage(`Login error: ${feature.failureMessage || AuthResult[feature.authResult]}`);
							reject(vscode.FileSystemError.Unavailable(connectHost));
						}
					} else {
						console.log(`ERR: Login(${remote.auth_key} ~ ${remote.username}) = ${JSON.stringify(feature)}`);
						vscode.window.showErrorMessage(`Login error: ${err?.details || "feature not present"}`);
						reject(vscode.FileSystemError.Unavailable(connectHost));
					}
				});
			});
		}
	}

	loginAndRepeat<T>(callback: () => Promise<T>): Promise<T> {
		return new Promise<T>((resolve, reject) => {
			this.login().then(() => resolve(callback())).catch(err => reject(err));
		});
	}

	checkError<T>(uri: vscode.Uri, retry: boolean, err: grpc.ServiceError, resolve: (value: T | PromiseLike<T>) => void, reject: (reason?: any) => void, retryCallback: () => Promise<T>): void {
		if (!retry && tokenExpired(err)) {
			resolve(this.loginAndRepeat(retryCallback));
		} else {
			reject(reportError(err, uri));
		}
	}

	stat(uri: vscode.Uri, retry: boolean = false): Promise<vscode.FileStat> {
		const statRequest: StatRequest = {
			authToken: this.auth_token,
			path: uri.path
		};
		return new Promise<vscode.FileStat>((resolve, reject) => {
			if (this.client == undefined) { return reject(vscode.FileSystemError.Unavailable(uri)); }
			this.client.Stat(statRequest, (err: grpc.ServiceError | null, feature: StatResponse__Output | undefined) => {
				if (!err) {
					console.log(`OK : Stat(${uri.authority} + ${uri.path}) = ${JSON.stringify(feature)}`);
					const fsEntry:VsFsEntry__Output = feature?.fileInfo as VsFsEntry__Output;
					const fileStat: vscode.FileStat = FileStat.fromVsFsEntry(fsEntry);
					resolve(fileStat);
				} else {
					console.log(`ERR: Stat(${uri.authority} + ${uri.path}) = ${err}`);
					this.checkError(uri, retry, err, resolve, reject, () => this.stat(uri, true));
				}
			});
		});
	}

	readDirectory(uri: vscode.Uri, retry: boolean = false): Promise<[string, vscode.FileType][]> {
		const listRequest: ListDirectoryRequest = {
			authToken: this.auth_token,
			path: uri.path
		};
		return new Promise<[string, vscode.FileType][]>((resolve, reject) => {
			if (this.client == undefined) { return reject(vscode.FileSystemError.Unavailable(uri)); }
			this.client.ListDirectory(listRequest, (err: grpc.ServiceError | null, feature: ListDirectoryResponse__Output | undefined) => {
				if (!err) {
					console.log(`OK : ListDirectory(${uri.authority} + ${uri.path}) = ${JSON.stringify(feature)}`);
					const fsEntries:VsFsEntry__Output[] = feature?.entries || [];
					const filesList: [string, vscode.FileType][] = fsEntries.map(fse => [fse.name, fsEntryTypeToVsCodeType(fse.fileType)]);
					resolve(filesList);
				} else {
					console.log(`ERR: ListDirectory(${uri.authority} + ${uri.path}) = ${err}`);
					this.checkError(uri, retry, err, resolve, reject, () => this.readDirectory(uri, true));
				}
			});
		});
	}

	readFile(uri: vscode.Uri, retry: boolean = false): Promise<Uint8Array> {
		const readFileRequest: ReadFileRequest = {
			authToken: this.auth_token,
			path: uri.path
		};
		return new Promise<Uint8Array>((resolve, reject) => {
			if (this.client == undefined) { return reject(vscode.FileSystemError.Unavailable(uri)); }
			this.client.ReadFile(readFileRequest, (err: grpc.ServiceError | null, feature: ReadFileResponse__Output | undefined) => {
				if (!err) {
					const content:Uint8Array = feature?.content || new Uint8Array();
					console.log(`OK : ReadFile: ${content.length} bytes read`);
					resolve(content);
				} else {
					console.log(`ERR: ReadFile(${uri.path}) = ${err}`);
					this.checkError(uri, retry, err, resolve, reject, () => this.readFile(uri, true));
				}
			});
		});
	}

	writeFile(uri: vscode.Uri, content: Uint8Array, options: { create: boolean, overwrite: boolean }, retry: boolean = false): Promise<void> {
		const writeFileRequest: WriteFileRequest = {
			authToken: this.auth_token,
			path: uri.path,
			content: content,
			create: options.create,
			overwrite: options.overwrite
		};
		return new Promise<void>((resolve, reject) => {
			if (this.client == undefined) { return reject(vscode.FileSystemError.Unavailable(uri)); }
			this.client.WriteFile(writeFileRequest, (err: grpc.ServiceError | null, feature: WriteFileResponse__Output | undefined) => {
				if (!err) {
					console.log(`OK : WriteFile(${uri.path}) = ${JSON.stringify(feature)}`);
					resolve();
					this._fireSoon({ type: vscode.FileChangeType.Changed, uri });
				} else {
					console.log(`ERR: WriteFile(${uri.path}) = ${err}`);
					this.checkError(uri, retry, err, resolve, reject, () => this.writeFile(uri, content, options, true));
				}
			});
		});
	}

	rename(oldUri: vscode.Uri, newUri: vscode.Uri, options: { overwrite: boolean }, retry: boolean = false): Promise<void> {
		const renameRequest: RenameFileRequest = {
			authToken: this.auth_token,
			fromPath: oldUri.path,
			toPath: newUri.path,
			overwrite: options.overwrite
		};
		return new Promise<void>((resolve, reject) => {
			if (this.client == undefined) { return reject(vscode.FileSystemError.Unavailable(oldUri)); }
			this.client.RenameFile(renameRequest, (err: grpc.ServiceError | null, feature: RenameFileResponse__Output | undefined) => {
				if (!err) {
					console.log(`OK : RenameFile(${oldUri.path} => ${newUri.path}) = ${JSON.stringify(feature)}`);
					resolve();
					this._fireSoon(
						{ type: vscode.FileChangeType.Deleted, uri: oldUri },
						{ type: vscode.FileChangeType.Created, uri: newUri }
					);
				} else {
					console.log(`ERR: RenameFile(${oldUri.path} => ${newUri.path}) = ${err}`);
					this.checkError(oldUri, retry, err, resolve, reject, () => this.rename(oldUri, newUri, options, true));
				}
			});
		});
	}

	delete(uri: vscode.Uri, options: { recursive: boolean}, retry: boolean = false): Promise<void> {
		const dirname = uri.with({ path: path.posix.dirname(uri.path) });
		return new Promise<void>((resolve, reject) => {
			this.stat(uri).then( fstat => {
				if (this.client == undefined) { return reject(vscode.FileSystemError.Unavailable(uri)); }
				if (fstat.type == vscode.FileType.Directory) {
					const removeDirectoryRequest: RemoveDirectoryRequest = {
						authToken: this.auth_token,
						path: uri.path,
						recursive: options.recursive
					};
					this.client.RemoveDirectory(removeDirectoryRequest, (err: grpc.ServiceError | null, feature: RemoveDirectoryResponse__Output | undefined) => {
						if (!err) {
							console.log(`OK : RemoveDirectory(${uri.path}) = ${JSON.stringify(feature)}`);
							resolve();
							this._fireSoon({ type: vscode.FileChangeType.Changed, uri: dirname }, { uri, type: vscode.FileChangeType.Deleted });
						} else {
							console.log(`ERR: RemoveDirectory(${uri.path}) = ${err}`);
							this.checkError(uri, retry, err, resolve, reject, () => this.delete(uri, options, true));
						}
					});
				} else {
					const deleteRequest: DeleteFileRequest = {
						authToken: this.auth_token,
						path: uri.path
					};
					this.client.DeleteFile(deleteRequest, (err: grpc.ServiceError | null, feature: DeleteFileResponse__Output | undefined) => {
						if (!err) {
							console.log(`OK : DeleteFile(${uri.path}) = ${JSON.stringify(feature)}`);
							resolve();
							this._fireSoon({ type: vscode.FileChangeType.Changed, uri: dirname }, { uri, type: vscode.FileChangeType.Deleted });
						} else {
							console.log(`ERR: DeleteFile(${uri.path}) = ${err}`);
							this.checkError(uri, retry, err, resolve, reject, () => this.delete(uri, options, true));
						}
					});
				}
			}).catch(err => reject(err));
		});
	}

	createDirectory(uri: vscode.Uri, retry: boolean = false): Promise<void> {
		const dirname = uri.with({ path: path.posix.dirname(uri.path) });
		const mkdirRequest: CreateDirectoryRequest = {
			authToken: this.auth_token,
			path: uri.path
		};
		return new Promise<void>((resolve, reject) => {
			if (this.client == undefined) { return reject(vscode.FileSystemError.Unavailable(uri)); }
			this.client.CreateDirectory(mkdirRequest, (err: grpc.ServiceError | null, feature: CreateDirectoryResponse__Output | undefined) => {
				if (!err) {
					console.log(`OK : CreateDirectory(${uri.path}) = ${JSON.stringify(feature)}`);
					resolve();
					this._fireSoon({ type: vscode.FileChangeType.Changed, uri: dirname }, { type: vscode.FileChangeType.Created, uri });
				} else {
					console.log(`ERR: CreateDirectory(${uri.path}) = ${err}`);
					this.checkError(uri, retry, err, resolve, reject, () => this.createDirectory(uri, true));
				}
			});
		});
	}

	// --- manage file events

	private _emitter = new vscode.EventEmitter<vscode.FileChangeEvent[]>();
	private _bufferedEvents: vscode.FileChangeEvent[] = [];
	private _fireSoonHandle?: NodeJS.Timer;

	readonly onDidChangeFile: vscode.Event<vscode.FileChangeEvent[]> = this._emitter.event;

	watch(_resource: vscode.Uri): vscode.Disposable {
		// ignore, fires for all changes...
		return new vscode.Disposable(() => { });
	}

	private _fireSoon(...events: vscode.FileChangeEvent[]): void {
		this._bufferedEvents.push(...events);

		if (this._fireSoonHandle) {
			clearTimeout(this._fireSoonHandle);
		}

		this._fireSoonHandle = setTimeout(() => {
			this._emitter.fire(this._bufferedEvents);
			this._bufferedEvents.length = 0;
		}, 5);
	}
}
