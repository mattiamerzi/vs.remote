'use strict';

import * as vscode from 'vscode';
import { VsRemoteFs } from './fileSystemProvider';
import { VsRemoteSettings } from './settings';
import { CommandParameterValidation } from './vsremote/CommandParameterValidation';
import { CommandTarget } from './vsremote/CommandTarget';

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
		console.log('WS:', vscode.workspace.workspaceFolders);
		console.log('WS1:' + vscode.workspace.workspaceFolders?.length);
		let uri = null;
		const activeTextEditor = vscode.window.activeTextEditor;
		let onFile = activeTextEditor != null;
		if (activeTextEditor != null) {
			uri = activeTextEditor.document.uri;
		} else {
			if (vscode.workspace.workspaceFolders != null && vscode.workspace.workspaceFolders.length > 0)
				uri = vscode.workspace.workspaceFolders[0].uri;
		}
		if (uri != null) {
			const { authority, path } = uri;
			console.log(`currently open file authority: ${authority}`)
			console.log(`currently open file path: ${path}`);
			const settings = new VsRemoteSettings();
			const remote = settings.getRemote(authority);
			if (remote == undefined)
				return;
			const vsRemote = VsRemoteFs.Instance;
			let remoteCommands = await vsRemote.listRemoteCommands(remote);
			if (!onFile)
				remoteCommands = remoteCommands?.filter(cmd => cmd.target == CommandTarget.NO_TARGET) || null;
			if (!remoteCommands)
				return;
			let picked_cmd = await vscode.window.showQuickPick(
				remoteCommands.map(cmd => {
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
			let dirtyop: string|null|undefined = null;
			const save = "Save changes";
			const discard = "Discard changes";
			const cancel = "Cancel command";
			if (onFile && picked_cmd.command.modifies_file_content && activeTextEditor != null &&  activeTextEditor.document.isDirty) {
				dirtyop = await vscode.window.showWarningMessage("Chosen command might modify the currently active document, but the document has unsaved changes, proceed anyway?",
					save, discard, cancel);
				if (dirtyop == undefined || dirtyop == cancel)
					return;
			}
			let pvalue: string|undefined = undefined;
			for (let param of picked_cmd.command.parameters) {
				let valid: boolean = false;
				while (!valid) {
					pvalue = await vscode.window.showInputBox({
						prompt: `${param.description}`
					})
					if (pvalue == undefined)
						return;
					switch (param.validation) {
						case CommandParameterValidation.NONE:
							valid = true;
							break;
						case CommandParameterValidation.NON_EMPTY:
							if (pvalue.trim().length == 0) {
								await vscode.window.showErrorMessage(`Empty or blank string is not valid for parameter ${param.name}`, { modal: true });
								valid = false;
							}
							else {
								valid = true;
								pvalue = pvalue.trim();
							}
							break;
						case CommandParameterValidation.INTEGER:
							let intvalue = parseInt(pvalue);
							if (isNaN(intvalue)) {
								await vscode.window.showErrorMessage(`Parameter ${param.name} must be a valid number`, { modal: true });
								valid = false;
							} else {
								pvalue = intvalue.toString();
								valid = true;
							}
							break;
					}
				}
				param.value = pvalue as string;
			}
			if (onFile && dirtyop != null) {
				switch (dirtyop) {
					case save:
						await activeTextEditor?.document.save();
						break;
					case discard:
						await vscode.commands.executeCommand('workbench.action.files.revert');
						break;
					default:
						return;
				}
			}
			const cmdresult = await vsRemote.executeRemoteCommand(onFile ? activeTextEditor?.document.uri : null, remote, picked_cmd.command);
			if (cmdresult.success) {
				vscode.window.showInformationMessage(cmdresult.message, { modal: false });
			} else {
				vscode.window.showErrorMessage(cmdresult.message, { modal: false });
			}
			if (picked_cmd.command.modifies_file_content)
				await vscode.commands.executeCommand('workbench.action.files.revert'); // works as "reload"
		}
	}));
}

export function deactivate() {
	console.log('Vs.Remote: deactivate()');
}
