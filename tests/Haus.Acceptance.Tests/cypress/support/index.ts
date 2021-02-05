import './commands'
import {INTERCEPTORS} from "./interceptors";

before(() => {
    Object.keys(INTERCEPTORS)
        .forEach(key => cy.intercept(INTERCEPTORS[key].url).as(INTERCEPTORS[key].name));
    
    cy.login();
});
