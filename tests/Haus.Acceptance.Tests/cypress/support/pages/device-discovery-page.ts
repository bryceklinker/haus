import {DevicesPage} from "./devices-page";

export const DeviceDiscoveryPage = {
    navigate: () => {
        DevicesPage.navigate();
        cy.findByRole('button', {name: 'discovery'}).click();
    },
    findUnassignedDevices: () => cy.findAllByLabelText('unassigned device item')
}