export const HealthPage = {
    navigate: () => cy.navigate('Health'),
    findHeader: () => cy.findByLabelText('health header')
}