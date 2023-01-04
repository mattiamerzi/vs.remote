import * as vscode from 'vscode';

const DEFAULT_PORT:number = 8080;

export class VsRemoteHost {
    public name: string = '';
    public description: string = '';
    public host: string = '';
    public port: number = DEFAULT_PORT;
    public auth_key: string | undefined;
    public username: string | undefined;
    public password: string | undefined;
    public auth_method: string = 'none'; // one of 'none', 'auth_key', 'password'
}

export class VsRemoteSettings {

    get() {
        return vscode.workspace.getConfiguration('vs.remote');
    }

    getRemotes(): VsRemoteHost[] {
        const remlistobj: ({ [key: string]: VsRemoteHost} | undefined) = this.get().get('remotes');
        if (remlistobj === undefined) {
            return [];
        } else {
            const rems = remlistobj;
            Object.keys(remlistobj).forEach(k => rems[k].name = k);
            return Object.keys(remlistobj).map(k => rems[k]);
        }
    }

    getRemote(name: string) {
        return this.getRemotes().find(r => r.name == name);
    }

    async chooseRemote(): Promise<VsRemoteHost | null> {
        const remotes = this.getRemotes();
        const qp = await vscode.window.showQuickPick(
            remotes.map(r => {
                const hostport = `${r.host}:${r.port}`;
                const description = r.description ? `${r.description} (${hostport})` : hostport;
                return {
                    label: r.name,
                    description,
                    remote: r
                };
            }),
            {
                placeHolder: 'Choose a Vs.Remote enabled host'
            }
        );
        if (qp === undefined) {
            return null;
        } else {
            return qp.remote;
        }
    }

    async checkAuth(remote: VsRemoteHost): Promise<boolean> {
        if (remote.auth_key && (remote.username || remote.password)) {
           vscode.window.showErrorMessage("Configuration error: one of auth_key or username / password must be specified");
           return false;
        }
        if (remote.auth_key) {
            remote.auth_method = 'auth_key';
            return true;
        }
        if (remote.password && !remote.username) {
           vscode.window.showErrorMessage("Configuration error: missing username");
           return false;
        }
        if (remote.username) {
            if (!remote.password) {
                const tmppasswd = await vscode.window.showInputBox({
                    ignoreFocusOut: true,
                    password: true,
                    prompt: `Insert password for ${remote.name} (${remote.host}:${remote.port})`
                });
                if (tmppasswd == null) {
                    return false;
                } else {
                    remote.password = tmppasswd;
                    remote.auth_method = 'password';
                    return true;
                }
            } else {
                remote.auth_method = 'password';
                return true;
            }
        }
        remote.auth_method = 'none';
        return true;
    }

}