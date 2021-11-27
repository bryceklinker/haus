export interface InterceptorConfig {
    name: string;
    url: string;
    alias: string;
    method: Cypress.HttpMethod;
}

export const API_ALIASES = {
    GET_SETTINGS: '@settings',
    ADD_ROOM: '@addRoom'
}

export const INTERCEPTORS = {
    settings: {
        name: 'settings',
        url: '/client-settings',
        alias: API_ALIASES.GET_SETTINGS,
        method: 'GET',
    } as InterceptorConfig,
    addRoom: {
        name: 'addRoom',
        url: '/api/rooms',
        method: 'POST',
        alias: API_ALIASES.ADD_ROOM
    } as InterceptorConfig,
}

export function waitForApi(alias: string) {
    cy.wait(alias);
}