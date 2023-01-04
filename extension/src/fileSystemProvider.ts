import * as vscode from 'vscode';
import { VsRemoteHost } from './settings';
import { VsRemoteFsProvider } from './remoteFilesystem';

export class VsRemoteFs implements vscode.FileSystemProvider {
	private fss: Map<string, VsRemoteFsProvider> = new Map<string, VsRemoteFsProvider>();

	constructor() {
	}

	onFs<T>(key: string, action: (fs: VsRemoteFsProvider) => T): T {
//		console.log(`onFs(${key}) = ${action.toString()}`);
		const fs: VsRemoteFsProvider | undefined = this.fss.get(key);
		if (fs)
			return action(fs);
		throw vscode.FileSystemError.Unavailable(key);
	}

	connect(remote: VsRemoteHost): void {
		console.log(`Vs.Remote connecting to ${remote.name} (${remote.host}:${remote.port})`);
		this.fss.set(remote.name, new VsRemoteFsProvider(remote));
	}

	alreadyConnected(uri: vscode.Uri): boolean {
		return this.fss.has(uri.authority);
	}

	login(remote: VsRemoteHost): Promise<boolean> {
		try {
			return this.onFs(remote.name, fs => fs.login());
		} catch {
			return Promise.resolve(false);
		}
	}

	stat(uri: vscode.Uri): Promise<vscode.FileStat> {
		return this.onFs(uri.authority, fs => fs.stat(uri));
	}

	readDirectory(uri: vscode.Uri): Promise<[string, vscode.FileType][]> {
		return this.onFs(uri.authority, fs => fs.readDirectory(uri));
	}

	readFile(uri: vscode.Uri): Promise<Uint8Array> {
		return this.onFs(uri.authority, fs => fs.readFile(uri));
	}

	writeFile(uri: vscode.Uri, content: Uint8Array, options: { create: boolean, overwrite: boolean }): Promise<void> {
		return this.onFs(uri.authority, fs => fs.writeFile(uri, content, options));
	}

	rename(oldUri: vscode.Uri, newUri: vscode.Uri, options: { overwrite: boolean }): Promise<void> {
		return this.onFs(oldUri.authority, fs => fs.rename(oldUri, newUri, options));
	}

	delete(uri: vscode.Uri, options: { recursive: boolean}): Promise<void> {
		return this.onFs(uri.authority, fs => fs.delete(uri, options));
	}

	createDirectory(uri: vscode.Uri): Promise<void> {
		return this.onFs(uri.authority, fs => fs.createDirectory(uri));
	}

	private _emitter = new vscode.EventEmitter<vscode.FileChangeEvent[]>();

	readonly onDidChangeFile: vscode.Event<vscode.FileChangeEvent[]> = this._emitter.event;

	watch(_resource: vscode.Uri): vscode.Disposable {
		// ignore, fires for all changes...
		return new vscode.Disposable(() => { });
	}

}
