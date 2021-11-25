export const DeviceSimulatorPage = {
    navigate: () => cy.navigate('Device Simulator'),
    findHeader: () => cy.findByTestId('device-simulator-header')
}