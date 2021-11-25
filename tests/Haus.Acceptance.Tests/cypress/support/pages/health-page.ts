export const HealthPage = {
    navigate: () => cy.navigate('Health'),
    findHeader: () => cy.findByTestId('health-header')
}