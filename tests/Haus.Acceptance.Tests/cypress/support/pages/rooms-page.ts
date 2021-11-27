export const RoomsPage = {
    navigate: () => cy.navigate('Rooms'),
    findHeader: () => cy.findByLabelText('rooms header'),
    findDevices: () => cy.findAllByLabelText('room device item'),
    selectRoom: (name: string) => cy.findAllByLabelText('room item').findByText(name).click(),
    expandDevices: () => cy.findByRole('button', { name: 'Room Devices' }).click(),
    addRoom: (name: string) => {
        cy.findByRole('button', {name: 'add room'}).click();
        cy.findByRole('textbox', {name: 'room name'}).type(name);
        cy.findByRole('button', {name: 'save room'}).click();
    },
    assignDevicesToRoom: (...deviceIds: Array<string>) => {
        cy.findByRole('button', {name: 'assign devices'}).click();
        deviceIds.forEach(deviceId => {
            cy.findAllByLabelText('unassigned device item')
                .findByText(deviceId)
                .click();    
        });
        cy.findByRole('button', {name: 'assign devices to room'}).click();
    }
}