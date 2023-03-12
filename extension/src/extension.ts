'use strict';

import * as vscode from 'vscode';
import { VsRemoteFs } from './fileSystemProvider';
import { VsRemoteCommand } from './remoteFilesystem';
import { VsRemoteSettings } from './settings';

export async function activate(context: vscode.ExtensionContext) {
	console.log('Vs.Remote says "Hello, World!"');

	if (vscode.workspace.workspaceFolders && vscode.workspace.workspaceFolders.length > 0) {
		const vsRemote = new VsRemoteFs();
		context.subscriptions.push(vscode.workspace.registerFileSystemProvider('vsrem', vsRemote, { isCaseSensitive: true }));

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
						let remoteCommands = await vsRemote.listRemoteCommands(remote);
						if (remoteCommands && remoteCommands?.length > 0) {
							vscode.commands.executeCommand('setContext', 'vsrem.hasVsRemoteCommands', true);
						}
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

	context.subscriptions.push(vscode.commands.registerCommand('vsrem.showVsRemoteCommands', async() => {
		console.log('in vsrem.showVsRemoteCommands');
		const activeTextEditor = vscode.window.activeTextEditor;
		if (activeTextEditor != null) {
			const { authority, path } = activeTextEditor.document.uri;
			console.log(`currently open file authority: ${authority}`)
			console.log(`currently open file path: ${path}`);
			const settings = new VsRemoteSettings();
			const remote = settings.getRemote(authority);
			if (remote == undefined)
				return;
			const vsRemote = VsRemoteFs.Instance;
			const remoteCommands = await vsRemote.listRemoteCommands(remote);
			if (remoteCommands == undefined)
				return;
			let picked_cmd = await vscode.window.showQuickPick(
				remoteCommands?.map(cmd => {
					return {
					label: cmd.name,
					description: cmd.description,
					command: cmd
				}}),
				{
					placeHolder: 'Choose a Vs.Remote Command to execute'
				}
			);
			if (picked_cmd == undefined)
				return;
			for (let param of picked_cmd.command.parameters) {
				let pvalue = await vscode.window.showInputBox({
					prompt: `${param.description}`
				})
				if (pvalue == undefined)
					return;
				param.value = pvalue;
			}
			const cmdresult = await vsRemote.executeRemoteCommand(remote, picked_cmd.command);
			if (cmdresult.success) {
				vscode.window.showInformationMessage(cmdresult.message, { modal: true });
			} else {
				vscode.window.showErrorMessage(cmdresult.message, { modal: true });
			}
		}
	}));
}

export function deactivate() {
	console.log('Vs.Remote: deactivate()');
}
