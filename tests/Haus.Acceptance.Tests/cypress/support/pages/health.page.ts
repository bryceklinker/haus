export class HealthPage {
    static navigate() {
        cy.navigate('Health');
    }
    
    static getDashboard() {
        return cy.getByTestId('health-dashboard');
    }
    
    static getRecentEvents() {
        return cy.getByTestId('recent-events');
    }
    
    static getRecentLogs() {
        return cy.getByTestId('recent-logs');
    }
}