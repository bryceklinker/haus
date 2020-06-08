Cypress.Commands.add('login', ({username, password}) => {
    cy.visit('/');
    cy.getByTestId('username-input').type(username);
    cy.getByTestId('password-input').type(password);
    cy.getByTestId('login-button').click();
})