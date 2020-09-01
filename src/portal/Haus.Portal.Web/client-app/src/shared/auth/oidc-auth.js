import {UserManager} from "oidc-client";

export function createUserManager(settings) {
    return new UserManager({
        ...settings,
        redirect_uri: window.origin
    });
}

export async function ensureUserSignedIn(userManager) {
    if (isErrorCallback()) {
        throw new Error('An error occurred during signin');
    }

    const user = isSigninRedirectCallback(userManager)
        ? await userManager.signinRedirectCallback()
        : await getCurrentUser(userManager);

    if (user) {
        removeSensitiveDataFromUri();
        return user;
    }

    await userManager.clearStaleState();
    await userManager.signinRedirect();
    return null;
}

function isSigninRedirectCallback(userManager) {
    const params = new URLSearchParams(window.location.search);
    return params.has(userManager.settings.response_type);
}

function isErrorCallback() {
    const params = new URLSearchParams(window.location.search);
    return params.has('error');
}

function removeSensitiveDataFromUri() {
    const currentUri = window.location.toString();
    if (currentUri.indexOf('?') > 0) {
        const scrubbedUri = currentUri.substring(0, currentUri.indexOf('?'));
        window.history.replaceState({}, document.title, scrubbedUri);    
    }
}

async function getCurrentUser(userManager) {
    const user = await userManager.getUser();
    if (!user) {
        return null;
    }

    return user.expired
        ? null
        : user;
}

