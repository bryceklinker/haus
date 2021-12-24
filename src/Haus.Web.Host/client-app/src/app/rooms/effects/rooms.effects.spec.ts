import {RoomsEffects} from './rooms.effects';
import {
  createAppTestingService,
  eventually,
  ModelFactory,
  setupAddRoom,
  setupAssignDevicesToRoom,
  setupChangeRoomLighting,
  setupGetAllRooms,
  setupUpdateRoom,
  TestingActionsSubject
} from '../../../testing';
import {RoomsActions} from '../state';

describe('RoomsEffects', () => {
  let actions$: TestingActionsSubject;

  beforeEach(() => {
    const {actionsSubject} = createAppTestingService(RoomsEffects);
    actions$ = actionsSubject;
  });

  test('when load rooms requested then gets rooms from api', async () => {
    const expected = ModelFactory.createListResult(
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel(),
    );
    setupGetAllRooms(expected.items);

    actions$.next(RoomsActions.loadRooms.request());

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(RoomsActions.loadRooms.success(expected));
    });
  });

  test('when add room requested then adds room to api', async () => {
    const expected = ModelFactory.createRoomModel();
    setupAddRoom(expected);

    actions$.next(RoomsActions.addRoom.request({name: expected.name}));

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(RoomsActions.addRoom.success(expected));
    });
  });

  test('when update room requested then updates room on api', async () => {
    const expected = ModelFactory.createRoomModel({id: 12});
    setupUpdateRoom(12);

    actions$.next(RoomsActions.updateRoom.request(expected));

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(RoomsActions.updateRoom.success(expected));
    });
  });

  test('when room lighting change requested then changes lighting for room', async () => {
    setupChangeRoomLighting(54);

    const lighting = ModelFactory.createLighting();
    const room = ModelFactory.createRoomModel({id: 54});
    actions$.next(RoomsActions.changeRoomLighting.request({room, lighting}));

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(RoomsActions.changeRoomLighting.success({room, lighting}));
    });
  });

  test('when assign devices to room requested then assigns devices to room', async () => {
    setupAssignDevicesToRoom(65);

    const deviceIds = [12, 5, 6];
    actions$.next(RoomsActions.assignDevicesToRoom.request({roomId: 65, deviceIds}));

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(RoomsActions.assignDevicesToRoom.success({
        roomId: 65,
        deviceIds
      }));
    });
  });
});
