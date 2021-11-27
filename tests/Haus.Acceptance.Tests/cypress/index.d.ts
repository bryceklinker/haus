declare namespace Cypress {
    interface Chainable {
        getToken(username?: string, password?: string): Chainable<Cypress.Response<import('./support/models').TokenResponse>>;
        login(username?: string, password?: string): Chainable;
        logout(): Chainable;
        navigate(text: string): Chainable;
        isUserLoggedIn(): Chainable<boolean>;
        waitForAppToBeReady(): Chainable;
        publishZigbeeMessage(message: import('./support/models').Zigbee2MqttMessage): Chainable<any>;
        waitForApi(alias: string): Chainable<any>;
    }
}