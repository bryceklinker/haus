export const DevicesPage = {
    navigate: () => cy.navigate('Devices'),
    findHeader: () => cy.findByTestId('devices-header')
}