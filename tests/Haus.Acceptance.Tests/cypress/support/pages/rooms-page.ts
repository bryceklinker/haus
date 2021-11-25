export const RoomsPage = {
    navigate: () => cy.navigate('Rooms'),
    findHeader: () => cy.findByTestId('rooms-header')
}