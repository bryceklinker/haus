export class SpyUserManager {
    constructor() {
        this.user = null;
        this.signinRedirect = jest.fn().mockReturnValue(Promise.resolve());
        this.signinRedirectCallback = jest.fn().mockImplementation(() => this.user);
    }
}