import {
  ModelFactory,
} from "../../../../testing";
import {RoomsRootComponent} from "./rooms-root.component";
import {RoomsActions} from "../../state";
import {AddRoomDialogComponent} from "../add-room-dialog/add-room-dialog.component";
import {RoomsRootHarness} from "./rooms-root.harness";

describe('RoomsRootComponent', () => {
  test('should load rooms when rendered', async () => {
    const harness = await RoomsRootHarness.render();

    expect(harness.dispatchedActions).toContainEqual(RoomsActions.loadRooms.request());
  })

  test('should show rooms when rendered', async () => {
    const rooms = [
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel()
    ];

    const harness = await RoomsRootHarness.render(RoomsActions.loadRooms.success(ModelFactory.createListResult(...rooms)));

    expect(harness.rooms).toHaveLength(3);
  })

  test('should open add dialog when add room clicked', async () => {
    const harness = await RoomsRootHarness.render();

    await harness.addRoom();

    expect(harness.dialog.open).toHaveBeenCalledWith(AddRoomDialogComponent);
  })
})
