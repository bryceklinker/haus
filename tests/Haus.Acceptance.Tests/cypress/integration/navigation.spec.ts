describe('Navigation', () => {
    it('should allow navigation through the app', () => {
        cy.login();
        
        cy.navigate('Diagnostics');
        cy.getByTestId('diagnostics-header').should('be.visible');
        
        cy.navigate('Devices');
        cy.getByTestId('devices-header').should('be.visible');
        
        cy.navigate('Rooms');
        cy.getByTestId('rooms-header').should('be.visible');
        
        cy.navigate('Device Simulator');
        cy.getByTestId('device-simulator-header').should('be.visible');
        
        cy.navigate('Health');
        cy.getByTestId('health-header').should('be.visible');
        
        cy.logout();
    })
})