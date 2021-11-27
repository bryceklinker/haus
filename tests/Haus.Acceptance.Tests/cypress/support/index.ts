import '@testing-library/cypress/add-commands';
import './commands'
import {INTERCEPTORS} from "./interceptors";

before(() => {
    Object.keys(INTERCEPTORS)
        .forEach(key => cy.intercept(INTERCEPTORS[key].method, INTERCEPTORS[key].url).as(INTERCEPTORS[key].name));
});

export * from './zigbee-2-mqtt-message-builder';
export * from './zigbee-2-mqtt-message-factory';
export * from './mqtt-client';
