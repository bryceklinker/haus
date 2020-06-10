Cypress.Commands.add('login', ({username, password}, {clientId,redirectUrl}) => {
    cy.visit(`/?client_id=${clientId}&redirect_uri=${encodeURIComponent(redirectUrl)}`);
    cy.getByTestId('username-input').type(username);
    cy.getByTestId('password-input').type(password);
    cy.getByTestId('login-button').click();
})