import * as jwt from 'jsonwebtoken'
import {AUTH_SETTINGS} from "./auth-settings";
import {INTERCEPTORS} from "./interceptors";
import {JwtPayload} from "jsonwebtoken";

const DEFAULT_WAIT_TIME_IN_MS = 1000;

Cypress.Commands.add('getToken', (username = AUTH_SETTINGS.username, password = AUTH_SETTINGS.password) => {
    const {domain, audience, client_id, client_secret} = AUTH_SETTINGS;
    Cypress.log({
        displayName: 'Getting Token',
        message: `Retrieving token from ${domain}`
    });
    return cy.request<Cypress.TokenResponse>({
        method: 'POST',
        url: `https://${domain}/oauth/token`,
        body: {
            grant_type: 'password',
            username,
            password,
            audience,
            scope: 'openid profile',
            client_id,
            client_secret
        }
    })
})

Cypress.Commands.add('login', (username = AUTH_SETTINGS.username, password = AUTH_SETTINGS.password) => {
    return cy.getToken(username, password).then(({body}) => {
        const {audience, client_id} = AUTH_SETTINGS;
        const claims = jwt.decode(body.id_token) as JwtPayload;
        const {
            nickname,
            name,
            picture,
            updated_at,
            email,
            email_verified,
            sub,
            exp,
        } = claims;

        const item = {
            body: {
                ...body,
                decodedToken: {
                    claims,
                    user: {
                        nickname,
                        name,
                        picture,
                        updated_at,
                        email,
                        email_verified,
                        sub,
                    },
                    audience,
                    client_id,
                },
            },
            expiresAt: exp,
        };

        window.localStorage.setItem('auth0-cypress', JSON.stringify(item));
        cy.visit('/');
    })    
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