import {RoomsEffects} from "./rooms.effects";
import {
  createAppTestingService,
  eventually,
  ModelFactory,
  setupAddRoom,
  setupAssignDevicesToRoom,
  setupChangeRoomLighting,
  setupGetAllRooms,
  TestingActionsSubject
} from "../../../testing";
import {RoomsActions} from "../state";

describe('RoomsEffects', () => {
  let actions$: TestingActionsSubject;

  beforeEach(() => {
    const {actionsSubject} = createAppTestingService(RoomsEffects);
    actions$ = actionsSubject;
  })

  it('should get rooms from api', async () => {
    const expected = ModelFactory.createListResult(
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel(),
    );
    setupGetAllRooms(expected.items);

    actions$.next(RoomsActions.loadRooms.request());

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(RoomsActions.loadRooms.success(expected));
    })
  })

  it('should add room to api', async () => {
    const expected = ModelFactory.createRoomModel();
    setupAddRoom(expected);

    actions$.next(RoomsActions.addRoom.request({name: expected.name}));

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(RoomsActions.addRoom.success(expected));
    })
  })

  it('should change room lighting when change room lighting requested', async () => {
    setupChangeRoomLighting(54);

    const lighting = ModelFactory.createLighting();
    actions$.next(RoomsActions.changeRoomLighting.request({roomId: 54, lighting}));

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(RoomsActions.changeRoomLighting.success({roomId: 54, lighting}));
    })
  })

  it('should assign devices to room when assign devices to room requested', async () => {
    setupAssignDevicesToRoom(65);

    const deviceIds = [12, 5, 6];
    actions$.next(RoomsActions.assignDevicesToRoom.request({roomId: 65, deviceIds}));

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(RoomsActions.assignDevicesToRoom.success({roomId: 65, deviceIds}));
    })
  })
})
