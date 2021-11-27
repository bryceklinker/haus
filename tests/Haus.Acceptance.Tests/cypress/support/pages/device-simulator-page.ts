export const DeviceSimulatorPage = {
    navigate: () => cy.navigate('Device Simulator'),
    findHeader: () => cy.findByLabelText('device simulator header')
}