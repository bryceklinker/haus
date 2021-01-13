import {ModelFactory, TestingEventEmitter} from "../../../../testing";
import {RoomsListComponent} from "./rooms-list.component";
import {RoomsListHarness} from "./rooms-list.harness";

describe('RoomsListComponent', () => {
  it('should show rooms', async () => {
    const rooms = [
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel(),
    ];

    const harness = await RoomsListHarness.render({rooms});

    expect(harness.rooms).toHaveLength(3);
  })

  it('should notify when room is added', async () => {
    const emitter = new TestingEventEmitter();

    const harness = await RoomsListHarness.render({addRoom: emitter});
    await harness.addRoom();

    expect(emitter.emit).toHaveBeenCalled();
  })
})
