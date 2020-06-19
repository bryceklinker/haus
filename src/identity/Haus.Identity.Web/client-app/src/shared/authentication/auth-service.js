import {UserManager, User} from "oidc-client";

const USER_STORAGE_KEY = 'haus-identity-user';

async function defaultCreateUserManager() {
    const response = await fetch('settings.json', {headers: {'Content-Type': 'application/json'}});
    const settings = await response.json();
    return new UserManager(settings);
}

export class AuthService {
    constructor(storage = localStorage, createUserManager = defaultCreateUserManager) {
        this.storage = storage;
        this.createUserManager = createUserManager;
        this.userManager = null;
        this.current_user = null;
    }

    getUser = () => {
        return this.current_user || (this.current_user = this.loadUserFromStorage());
    }

    startSignIn = async () => {
        const manager = await this.getUserManager();
        await manager.signinRedirect();
    }

    completeSignIn = async (url) => {
        const manager = await this.getUserManager();
        this.current_user = await manager.signinRedirectCallback(url);
        this.storage.setItem(USER_STORAGE_KEY, this.current_user.toStorageString());
    }

    isSignedIn = () => {
        return this.getUser() !== null;
    }

    getUserManager = async () => {
        return this.userManager || (this.userManager = await this.createUserManager());
    }

    loadUserFromStorage = () => {
        const json = this.storage.getItem(USER_STORAGE_KEY);
        return json ? User.fromStorageString(json) : null;
    }
}