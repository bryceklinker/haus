interface CreateRoomOptions {
    name: string;
}

export class RoomsPage {
    static navigate() {
        cy.visit('/rooms');
    }
    
    static createRoom({name}: CreateRoomOptions) {
        cy.getByTestId('add-room-btn').click();
        
        cy.getByTestId('add-room-dialog').should('exist');
        cy.getByTestId('room-name-field').type(name);
        cy.getByTestId('save-room-btn').click();
        cy.getByTestId('add-room-dialog').should('not.exist');
    }
    
    static navigateToDetail(name: string) {
        cy.getByTestId('room-item').contains(name).click();
    }
    
    static getDetail() {
        return cy.getByTestId('room-detail');
    }
}