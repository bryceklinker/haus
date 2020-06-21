import {When, Then} from "cypress-cucumber-preprocessor/steps";

let newUser = null;
When(/^I create a user$/, () => {
    newUser = {username: 'billy', password: 'abc123$'};
    cy.createUser(newUser);
})

Then(/^I should see user in list of users$/, () => {
    cy.getUsers().should('have.text', newUser.username);
})