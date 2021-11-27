export const DevicesPage = {
    navigate: () => cy.navigate('Devices'),
    findHeader: () => cy.findByLabelText('devices header')
}