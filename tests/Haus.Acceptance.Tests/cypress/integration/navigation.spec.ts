describe('Navigation', () => {
    it('should allow navigation through the app', () => {
        cy.login();
        
        cy.navigate('Diagnostics');
        cy.findByTestId('diagnostics-header').should('be.visible');
        
        cy.navigate('Devices');
        cy.findByTestId('devices-header').should('be.visible');
        
        cy.navigate('Rooms');
        cy.findByTestId('rooms-header').should('be.visible');
        
        cy.navigate('Device Simulator');
        cy.findByTestId('device-simulator-header').should('be.visible');
        
        cy.navigate('Health');
        cy.findByTestId('health-header').should('be.visible');
        
        cy.logout();
    })
})