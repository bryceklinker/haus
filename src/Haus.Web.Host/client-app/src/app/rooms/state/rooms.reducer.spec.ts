import {generateStateFromActions} from '../../../testing/app-state-generator';
import {roomsReducer} from './rooms.reducer';
import {RoomsActions} from './actions';
import {ModelFactory} from '../../../testing';
import {EventsActions} from '../../shared/events';

describe('roomsReducer', () => {
  test('when rooms loaded successfully then adds rooms to state', () => {
    const result = ModelFactory.createListResult(
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel()
    );
    const state = generateStateFromActions(roomsReducer, RoomsActions.loadRooms.success(result));

    expect(state.entities[result.items[0].id]).toEqual(result.items[0]);
    expect(state.entities[result.items[1].id]).toEqual(result.items[1]);
    expect(state.entities[result.items[2].id]).toEqual(result.items[2]);
  });

  test('when room added successfully then adds room to state', () => {
    const room = ModelFactory.createRoomModel();

    const state = generateStateFromActions(roomsReducer, RoomsActions.addRoom.success(room));

    expect(state.entities[room.id]).toEqual(room);
  });

  test('when room lighting change requested then updates room in state', () => {
    const room = ModelFactory.createRoomModel();
    const newLighting = ModelFactory.createLighting({temperature: ModelFactory.createTemperatureLighting()});

    const state = generateStateFromActions(roomsReducer,
      RoomsActions.loadRooms.success(ModelFactory.createListResult(room)),
      RoomsActions.changeRoomLighting.request({room, lighting: newLighting})
    );

    expect(state.entities[room.id]?.lighting).toEqual(newLighting);
  });

  test('when receives room lighting changed then updates room lighting in state', () => {
    const room = ModelFactory.createRoomModel();
    const lighting = ModelFactory.createLighting({level: ModelFactory.createLevelLighting()});

    const state = generateStateFromActions(roomsReducer,
      EventsActions.roomCreated({room}),
      EventsActions.roomLightingChanged({room, lighting})
    );

    expect(state.entities[room.id]?.lighting).toEqual(lighting);
  });

  test('when receives room created event then adds room to state', () => {
    const room = ModelFactory.createRoomModel();

    const state = generateStateFromActions(roomsReducer,
      EventsActions.roomCreated({room})
    );

    expect(state.entities[room.id]).toEqual(room);
  });

  test('when receives room updated event then updates room in state', () => {
    const room = ModelFactory.createRoomModel({id: 88});

    const state = generateStateFromActions(roomsReducer,
      EventsActions.roomCreated({room: ModelFactory.createRoomModel({id: 88})}),
      EventsActions.roomUpdated({room})
    );

    expect(state.entities[88]).toEqual(room);
  });

  test('when room updated then should update room in state', () => {
    const room = ModelFactory.createRoomModel({id: 44});

    const state = generateStateFromActions(roomsReducer,
      RoomsActions.addRoom.success(room),
      RoomsActions.updateRoom.request({...room, name: 'Bill'})
    );

    expect(state.entities[44]).toEqual({...room, name: 'Bill'});
  });
});
