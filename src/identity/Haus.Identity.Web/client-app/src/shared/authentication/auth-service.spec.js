import {AuthService} from "./auth-service";
import {User} from "oidc-client";
import {InMemoryStorage} from "../../testing/in-memory-storage";
import {SpyUserManager} from "../../testing/spy-user-manager";

let authService, inMemoryStorage, userManager = null;
beforeEach(() => {
    inMemoryStorage = new InMemoryStorage();
    userManager = new SpyUserManager();
    authService = new AuthService(inMemoryStorage, () => Promise.resolve(userManager));
});

test('when signing in then redirected to signin', async () => {
    await authService.startSignIn();
    
    expect(userManager.signinRedirect).toHaveBeenCalled();
});

test('when signin is compleeted then user is stored', async () => {
    const user = createUser();
    userManager.user = user;
    
    await authService.completeSignIn('https://something.com');
    
    expect(inMemoryStorage.getItem('haus-identity-user')).toEqual(user.toStorageString());
});

test('when user has completed signin then user is signed in', async () => {
    userManager.user = createUser();
    
    await authService.completeSignIn('');
    
    expect(authService.isSignedIn()).toEqual(true);
});

function createUser() {
    return new User({
        id_token: 'id',
        session_state: 'state',
        access_token: 'token',
        refresh_token: 'refresh',
        token_type: 'type',
        scope: 'scope',
        profile: {},
        expires_at: 123,
        state: ''
    });
}

