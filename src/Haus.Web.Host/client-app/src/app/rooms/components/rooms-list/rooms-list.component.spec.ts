import {screen} from "@testing-library/dom";
import userEvent from "@testing-library/user-event";

import {ModelFactory, renderFeatureComponent, TestingEventEmitter} from "../../../../testing";
import {RoomsListComponent} from "./rooms-list.component";
import {RoomsModule} from "../../rooms.module";

describe('RoomsListComponent', () => {
  it('should show rooms', async () => {
    const rooms = [
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel(),
    ];

    await renderComponent({rooms});

    expect(screen.queryAllByTestId('room-item')).toHaveLength(3);
  })

  it('should notify when room is added', async () => {
    const emitter = new TestingEventEmitter();

    await renderComponent({addRoom: emitter});
    userEvent.click(screen.getByTestId('add-room-btn'));

    expect(emitter.emit).toHaveBeenCalled();
  })

  function renderComponent({rooms = [], addRoom = new TestingEventEmitter()}: Partial<RoomsListComponent> = {}) {
    return renderFeatureComponent(RoomsListComponent, {
      imports: [RoomsModule],
      componentProperties: {rooms, addRoom}
    })
  }
})
