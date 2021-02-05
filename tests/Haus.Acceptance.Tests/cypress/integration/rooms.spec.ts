import {v4 as uuid} from 'uuid';
import {RoomsPage} from "../support/pages";

describe('Rooms', () => {
    beforeEach(() => {
        RoomsPage.navigate();
    })
    
    it('should create new room', () => {
        const roomName = uuid();
        
        RoomsPage.createRoom({name: roomName});
        RoomsPage.navigateToDetail(roomName);
        
        RoomsPage.getDetail().should('contain.text', roomName);
    })
})