'use strict';

import * as vscode from 'vscode';
import { VsRemoteFs } from './fileSystemProvider';
import { VsRemoteSettings } from './settings';

export async function activate(context: vscode.ExtensionContext) {
	console.log('Vs.Remote says "Hello, World!"');

	const vsRemote = new VsRemoteFs();
	context.subscriptions.push(vscode.workspace.registerFileSystemProvider('vsrem', vsRemote, { isCaseSensitive: true }));

	if (vscode.workspace.workspaceFolders && vscode.workspace.workspaceFolders.length > 0) {
		const unconnectedFolders: vscode.WorkspaceFolder[] = vscode.workspace.workspaceFolders.filter(folder => !vsRemote.alreadyConnected(folder.uri));
		for (const folder of unconnectedFolders) {
			try {
				console.log(`activate[${folder.uri}]`);
				const wsuri = folder.uri;
				const settings = new VsRemoteSettings();
				const remote = settings.getRemote(wsuri.authority);
				if (remote) {
					if (await settings.checkAuth(remote)) {
						vsRemote.connect(remote);
						vsRemote.login(remote);
					}
				}
			} catch { }
		}
	} else {
		console.log("activate: (none)");
	}

	context.subscriptions.push(vscode.commands.registerCommand('vsrem.addVsRemoteFolder', async() => {
		const settings = new VsRemoteSettings();
		console.log('Vs.Remote settings:');
		settings.getRemotes().forEach(r => {
			console.log(' - name:' + r.name + " @host: " + r.host);
		});
		const chrem = await settings.chooseRemote();
		console.log(' chosen:' + chrem?.name ?? '(none)');
		if (chrem) {
			vscode.workspace.updateWorkspaceFolders(0, 0, { uri: vscode.Uri.parse(`vsrem://${chrem?.name}/`), name: `${chrem?.name} ( vsrem://${chrem.host}:${chrem.port} )` });
		}
	}));
}

export function deactivate() {
	console.log('Vs.Remote: deactivate()');
}
