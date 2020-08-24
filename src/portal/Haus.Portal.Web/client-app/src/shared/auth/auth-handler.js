import {UserManager} from "oidc-client";

export const AUTH_STATE = {
    AUTHENTICATED: 'AUTHENTICATED',
    ERROR: 'ERROR',
    UNAUTHENTICATED: 'UNAUTHENTICATED'
};

function isResponseTypeCallback(responseType) {
    const params = new URLSearchParams(window.location.query);
    return params.has(responseType);
}

function isErrorCallback() {
    const params = new URLSearchParams(window.location.query);
    return params.has('error');
}

export async function ensureUserIsAuthenticated(userManager, responseType) {
    if (isResponseTypeCallback(responseType)) {
        return {state: AUTH_STATE.AUTHENTICATED, user: await userManager.signinRedirectCallback()};
    }

    if (isErrorCallback()) {
        return {state: AUTH_STATE.ERROR, user: null};
    }

    const user = await userManager.getUser()
    if (user && !user.expired) {
        return {state: AUTH_STATE.AUTHENTICATED, user};
    }
    
    await userManager.signinRedirect();
    return {state: AUTH_STATE.UNAUTHENTICATED, user: null};
}

export function createUserManager(settings) {
    return new UserManager({
        authority: settings.authority,
        client_id: settings.clientId,
        response_type: settings.responseType,
        redirect_uri: window.origin
    });
}