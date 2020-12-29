import {RoomsEffects} from "./rooms.effects";
import {
  createAppTestingService,
  eventually,
  ModelFactory, setupAddRoom,
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
})
