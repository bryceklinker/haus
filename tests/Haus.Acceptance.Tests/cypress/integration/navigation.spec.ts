import {DiagnosticsPage, DeviceSimulatorPage, HealthPage, RoomsPage, DevicesPage} from "../support/pages";

describe('Navigation', () => {
    it('should allow navigation through the app', () => {
        cy.login();
        
        DiagnosticsPage.navigate();
        DiagnosticsPage.findHeader().should('be.visible');
        
        DevicesPage.navigate();
        DevicesPage.findHeader().should('be.visible');
        
        RoomsPage.navigate();
        RoomsPage.findHeader().should('be.visible');
        
        DeviceSimulatorPage.navigate();
        DeviceSimulatorPage.findHeader().should('be.visible');
        
        HealthPage.navigate();
        HealthPage.findHeader().should('be.visible');
    })
})