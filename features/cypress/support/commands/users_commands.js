Cypress.Commands.add('createUser', ({username, password}) => {
   cy.visit('/users');
   cy.getByTestId('add-user-button').click();
   cy.getByTestId('username-input').type(username);
   cy.getByTestId('password-input').type(password);
   cy.getByTestId('save-user-button').click();
});

Cypress.Commands.add('getUsers', () => {
    cy.visit('/users');
    return cy.getByTestId('users-list');
})