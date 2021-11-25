export const AUTH_SETTINGS = {
    username: Cypress.env('AUTH_USERNAME'),
    password: Cypress.env('AUTH_PASSWORD'),
    client_id: Cypress.env('AUTH_CLIENT_ID'),
    client_secret: Cypress.env('AUTH_CLIENT_SECRET'),
    domain: Cypress.env('AUTH_DOMAIN'),
    audience: Cypress.env('AUTH_AUDIENCE'),
};