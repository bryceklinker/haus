declare namespace Cypress {
    interface Chainable {
        login(): Chainable;
        getByTestId(testId: string): Chainable<Element>;
    }
}