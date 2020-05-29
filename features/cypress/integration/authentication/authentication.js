import { Given, When, Then } from 'cypress-cucumber-preprocessor/steps';
import {ADMIN_USER} from "../../support/users";

let user = null;
Given(/^I have valid user credentials$/, () => {
    user = ADMIN_USER;
})

When(/^I login to HAUS$/, () => {
    cy.login(user);
})

Then(/^I should see the HAUS dashboard$/, () => {
    cy.getDashboard().should('be.visible');
})