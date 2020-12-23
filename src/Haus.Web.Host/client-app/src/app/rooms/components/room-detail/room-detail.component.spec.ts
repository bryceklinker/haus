import {RoomDetailComponent} from './room-detail.component';
import {ModelFactory, renderFeatureComponent} from "../../../../testing";
import {screen} from "@testing-library/dom";
import {RoomModel} from "../../../shared/rooms";
import {RoomsModule} from "../../rooms.module";

describe('RoomDetailComponent', () => {
  it('should show room name', async () => {
    const room = ModelFactory.createRoomModel({name: 'new hotness'});

    await renderComponent(room);

    expect(screen.getByTestId('room-detail')).toHaveTextContent('new hotness');
  })

  function renderComponent(room: RoomModel) {
    return renderFeatureComponent(RoomDetailComponent, {
      imports: [RoomsModule],
      componentProperties: {room}
    });
  }
});
