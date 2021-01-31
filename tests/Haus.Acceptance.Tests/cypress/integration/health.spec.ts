describe('Health', () => {
    beforeEach(() => {
        cy.visit('/');
    })
    
    it('should show health dashboard', () => {
        cy.visit('/health');
        
        cy.getByTestId('health-dashboard').should('be.visible');
        cy.getByTestId('recent-events').should('be.visible');
        cy.getByTestId('recent-logs').should('be.visible');
    })
})