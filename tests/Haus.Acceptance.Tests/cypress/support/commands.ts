import {AUTH_SETTINGS} from "./auth-settings";

Cypress.Commands.add('getByTestId', (testId) => cy.get(`[data-testid="${testId}"]`));

Cypress.Commands.add('login', () => {
    cy.visit('/');
    cy.get('[data-testid="haus-main"]').then((main) => {
        if (main.length > 0) {
            return;
        }
        
        cy.get('[name="email"]').type(AUTH_SETTINGS.username);
        cy.get('[name="password"]').type(AUTH_SETTINGS.password);
        cy.get('[name="submit"]').click();
    })
    cy.visit('/');
})
