import {DevicesPage} from "./devices-page";

export const DeviceDiscoveryPage = {
    navigate: () => {
        DevicesPage.navigate();
        cy.findByTestId('discovery').click();
    },
    findUnassignedDevices: () => cy.findAllByTestId('unassigned-device-item')
}