declare namespace Cypress {
    interface Chainable {
        login(): Chainable;
        logout(): Chainable;
        navigate(text: string): Chainable;
        isUserLoggedIn(): Chainable<boolean>;
        waitForAppToBeReady(): Chainable;
    }
}