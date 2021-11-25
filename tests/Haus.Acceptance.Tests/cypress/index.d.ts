declare namespace Cypress {
    interface TokenResponse {
        id_token: string;
        access_token: string;
    }
    
    interface Chainable {
        getToken(username?: string, password?: string): Chainable<Cypress.Response<TokenResponse>>;
        login(username?: string, password?: string): Chainable;
        logout(): Chainable;
        navigate(text: string): Chainable;
        isUserLoggedIn(): Chainable<boolean>;
        waitForAppToBeReady(): Chainable;
    }
}