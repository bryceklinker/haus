import * as jwt from 'jsonwebtoken'
import {AUTH_SETTINGS} from "./auth-settings";
import {API_ALIASES, INTERCEPTORS} from './interceptors';
import {JwtPayload} from "jsonwebtoken";
import {Zigbee2MqttMessage, TokenResponse} from './models';
import {MQTT_CLIENT} from './mqtt-client';

const DEFAULT_WAIT_TIME_IN_MS = 1000;

Cypress.Commands.add('getToken', (username = AUTH_SETTINGS.username, password = AUTH_SETTINGS.password) => {
    const {domain, audience, client_id, client_secret} = AUTH_SETTINGS;
    Cypress.log({
        displayName: 'Getting Token',
        message: `Retrieving token from ${domain}`
    });
    return cy.request<TokenResponse>({
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
    cy.findByRole('button', {name: 'user menu'}).click();
    return cy.findByRole('button', {name: 'logout'}).click();
})

Cypress.Commands.add('navigate', (text: string) => {
    cy.findByRole('button', {name: 'menu'}).click();
    cy.findAllByLabelText('nav link').contains(text).click();
    return cy.findByRole('button', {name: 'menu'}).click();
})

Cypress.Commands.add('isUserLoggedIn', () => {
    return cy.get('body').then(body => body.find('[aria-label="user menu"]').length > 0);
})

Cypress.Commands.add('waitForAppToBeReady', () => {
    cy.wait(INTERCEPTORS.settings.alias);
    cy.wait(DEFAULT_WAIT_TIME_IN_MS);
    return cy.findByLabelText('loading indicator').should('not.exist')
});

Cypress.Commands.add('publishZigbeeMessage', (message: Zigbee2MqttMessage) => {
    return MQTT_CLIENT.publishZigbee2MqttMessage(message);
})

Cypress.Commands.add('waitForApi', (alias: string) => {
    return cy.wait(alias);
})