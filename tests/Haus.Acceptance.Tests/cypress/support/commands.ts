import {AUTH_SETTINGS} from "./auth-settings";
import {INTERCEPTORS} from "./interceptors";
const DEFAULT_WAIT_TIME_IN_MS = 1000;

Cypress.Commands.add('login', () => {
    cy.visit('/');
    
    cy.waitForAppToBeReady();
    cy.isUserLoggedIn().then(isLoggedIn => {
        if (isLoggedIn) {
            return;
        }
        
        cy.wait(DEFAULT_WAIT_TIME_IN_MS);
        cy.get('[name="email"]').type(AUTH_SETTINGS.username);
        cy.get('[name="password"]').type(AUTH_SETTINGS.password);
        cy.get('[name="submit"]').click();
    });
    
    return cy.waitForAppToBeReady();
})

Cypress.Commands.add('logout', () => {
    cy.findByTestId('user-menu-btn').click();
    return cy.findByTestId('logout-btn').click();
})

Cypress.Commands.add('navigate', (text: string) => {
    cy.findByTestId('menu-btn').click();
    cy.findAllByTestId('nav-link').contains(text).click();
    return cy.findByTestId('menu-btn').click();
})

Cypress.Commands.add('isUserLoggedIn', () => {
    return cy.get('body').then(body => body.find('[data-testid="user-menu"]').length > 0);
})

Cypress.Commands.add('waitForAppToBeReady', () => {
    cy.wait(INTERCEPTORS.settings.alias);
    cy.wait(DEFAULT_WAIT_TIME_IN_MS);
    return cy.findByTestId('loading-indicator').should('not.exist')
});