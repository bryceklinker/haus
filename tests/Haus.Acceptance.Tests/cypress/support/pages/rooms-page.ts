import {API_ALIASES} from '../interceptors';

export const RoomsPage = {
    navigate: () => cy.navigate('Rooms'),
    findHeader: () => cy.findByLabelText('rooms header'),
    findDevices: () => cy.findAllByLabelText('room device item'),
    selectRoom: (name: string) => cy
        .findAllByRole('link', {name: 'room item'})
        .filter((_, e) => e.textContent.includes(name))
        .click(),
    expandDevices: () => cy.findByRole('button', { name: 'Room Devices' }).click(),
    addRoom: (name: string) => {
        cy.findByRole('button', {name: 'add room'}).click();
        cy.findByRole('textbox', {name: 'room name'}).type(name);
        cy.findByRole('button', {name: 'save room'}).click();
        cy.waitForApi(API_ALIASES.ADD_ROOM);
    },
    assignDevicesToRoom: (...deviceIds: Array<string>) => {
        cy.findByRole('button', {name: 'room devices'}).click();
        cy.findByRole('button', {name: 'assign devices'}).click();
        deviceIds.forEach(deviceId => {
            cy.findAllByLabelText('unassigned device item')
                .filter((_, e) => e.textContent.includes(deviceId))
                .click();
        });
        cy.findByRole('button', {name: 'assign devices to room'}).click();
    }
}