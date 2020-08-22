import {ADMIN_USER} from "../support";

describe('Login', () => {
    it('should allow user to login using a username and password', () => {
        cy.visit('/');
        cy.getByTestId('username-input').type(ADMIN_USER.username);
        cy.getByTestId('password-input').type(ADMIN_USER.password);
        cy.getByTestId('login-btn').click();
        cy.getByTestId('greeting').should('have.text', ADMIN_USER.name);
    })
})