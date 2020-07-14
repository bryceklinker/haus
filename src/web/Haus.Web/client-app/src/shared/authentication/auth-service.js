import {User, UserManager, WebStorageStateStore} from 'oidc-client';

export class AuthService {
    get client_id() {
        return this.settings.client_id;
    }

    get authority() {
        return this.settings.authority;
    }

    get userKey() {
        return `oidc.user:${this.authority}:${this.client_id}`
    }

    get settings() {
        return this.manager.settings;
    }

    constructor(settings) {
        this.manager = new UserManager({
            ...settings,
            userStore: new WebStorageStateStore({ store: window.sessionStorage })
        });
    }

    isSignedIn = () => {
        const storedUser = this.readStoredUser();
        return storedUser && !storedUser.expired;
    }

    getUser = () => {
        return this.manager.getUser();
    };

    startSignIn = async () => {
        return await this.manager.signinRedirect();
    };

    completeSignIn = async (url) => {
        return await this.manager.signinRedirectCallback();
    };

    readStoredUser = () => {
        const userJson = window.sessionStorage.getItem(this.userKey);
        return userJson
            ? User.fromStorageString(userJson)
            : null;
    };
}