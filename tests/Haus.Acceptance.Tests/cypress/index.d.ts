declare namespace Cypress {
    interface Chainable {
        login(): Chainable;
        logout(): Chainable;
        getByTestId(testId: string): Chainable<Element>;
        navigate(text: string): Chainable;
        isUserLoggedIn(): Chainable<boolean>;
        waitForAppToBeReady(): Chainable;
    }
}