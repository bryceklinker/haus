import {generateStateFromActions} from "../../../testing/app-state-generator";
import {roomsReducer} from "./rooms.reducer";
import {RoomsActions} from "./actions";
import {ModelFactory} from "../../../testing";
import {EventsActions} from "../../shared/events";

describe('roomsReducer', () => {
  it('should have all rooms in state', () => {
    const result = ModelFactory.createListResult(
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel()
    )
    const state = generateStateFromActions(roomsReducer, RoomsActions.loadRooms.success(result));

    expect(state.entities[result.items[0].id]).toEqual(result.items[0]);
    expect(state.entities[result.items[1].id]).toEqual(result.items[1]);
    expect(state.entities[result.items[2].id]).toEqual(result.items[2]);
  })

  it('should add room to rooms when add is successful', () => {
    const room = ModelFactory.createRoomModel();

    const state = generateStateFromActions(roomsReducer, RoomsActions.addRoom.success(room));

    expect(state.entities[room.id]).toEqual(room);
  })

  it('should update room lighting when room lighting change requested', () => {
    const room = ModelFactory.createRoomModel();
    const newLighting = ModelFactory.createLighting({temperature: 4500});

    const state = generateStateFromActions(roomsReducer,
      RoomsActions.loadRooms.success(ModelFactory.createListResult(room)),
      RoomsActions.changeRoomLighting.request({room, lighting: newLighting})
    );

    expect(state.entities[room.id]?.lighting).toEqual(newLighting);
  })

  it('should update lighting on room when room lighting changed event received', () => {
    const room = ModelFactory.createRoomModel();
    const lighting = ModelFactory.createLighting({level: 89});

    const state = generateStateFromActions(roomsReducer,
      EventsActions.roomLightingChanged({room, lighting})
    );

    expect(state.entities[room.id]?.lighting).toEqual(lighting);
  })

  it('should add room when room created event received', () => {
    const room = ModelFactory.createRoomModel();

    const state = generateStateFromActions(roomsReducer,
      EventsActions.roomCreated({room})
    );

    expect(state.entities[room.id]).toEqual(room);
  })

  it('should update room when room updated event received', () => {
    const room = ModelFactory.createRoomModel();

    const state = generateStateFromActions(roomsReducer,
      EventsActions.roomUpdated({room})
    );

    expect(state.entities[room.id]).toEqual(room);
  })
})
