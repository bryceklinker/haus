import {AUTH_SETTINGS} from "./auth-settings";
import {testIdSelector} from "./test-id-selector";
import {INTERCEPTORS} from "./interceptors";
const DEFAULT_WAIT_TIME_IN_MS = 1000;

Cypress.Commands.add('getByTestId', (testId: string) => cy.get(testIdSelector(testId)));

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
    
    cy.waitForAppToBeReady();
})

Cypress.Commands.add('logout', () => {
    cy.getByTestId('user-menu-btn').click();
    cy.getByTestId('logout-btn').click();
})

Cypress.Commands.add('navigate', (text: string) => {
    cy.getByTestId('menu-btn').click();
    cy.getByTestId('nav-link').contains(text).click();
    cy.getByTestId('menu-btn').click();
})

Cypress.Commands.add('isUserLoggedIn', () => {
    return cy.get('body').then(body => body.find(testIdSelector('user-menu')).length > 0);
})

Cypress.Commands.add('waitForAppToBeReady', () => {
    cy.wait(INTERCEPTORS.settings.alias);
    cy.wait(DEFAULT_WAIT_TIME_IN_MS);
    cy.getByTestId('loading-indicator').should('not.exist')
});