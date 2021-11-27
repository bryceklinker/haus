import {eventually, ModelFactory} from "../../../../testing";
import {RoomsActions} from "../../state";
import {DevicesActions} from "../../../devices/state";
import {LightingState, RoomModel} from "../../../shared/models";
import {AssignDevicesToRoomDialogComponent} from "../assign-devices-to-room-dialog/assign-devices-to-room-dialog.component";
import {RoomDetailRootHarness} from "./room-detail-root.harness";

describe('RoomDetailRootComponent', () => {
  let room: RoomModel;

  beforeEach(() => {
    room = ModelFactory.createRoomModel({
      lighting: ModelFactory.createLighting({state: LightingState.Off})
    });
  })


  it('should show room name', async () => {
    const harness = await RoomDetailRootHarness.render(room.id, RoomsActions.loadRooms.success(ModelFactory.createListResult(room)));

    await eventually(() => {
      expect(harness.container).toHaveTextContent(room.name);
    })
  })

  it('should load devices when rendered', async () => {
    const harness = await RoomDetailRootHarness.render();

    expect(harness.dispatchedActions).toContainEqual(DevicesActions.loadDevices.request());
  })

  it('should show devices in room', async () => {
    const harness = await RoomDetailRootHarness.render(
      room.id,
      RoomsActions.loadRooms.success(ModelFactory.createListResult(room)),
      DevicesActions.loadDevices.success(ModelFactory.createListResult(
        ModelFactory.createDeviceModel({roomId: room.id}),
        ModelFactory.createDeviceModel({roomId: room.id}),
      ))
    );

    await eventually(() => {
      expect(harness.devices).toHaveLength(2);
    })
  })

  it('should request lighting change when room lighting changed', async () => {
    const harness = await RoomDetailRootHarness.render(room.id, RoomsActions.loadRooms.success(ModelFactory.createListResult(room)));

    harness.turnRoomOn();

    expect(harness.dispatchedActions).toContainEqual(RoomsActions.changeRoomLighting.request({
      room: room,
      lighting: expect.objectContaining({state: LightingState.On})
    }));
  })

  it('should open assign devices dialog when assign devices triggered', async () => {
    const harness = await RoomDetailRootHarness.render(room.id, RoomsActions.loadRooms.success(ModelFactory.createListResult(room)));

    await harness.assignDevices();

    expect(harness.dialog.open).toHaveBeenCalledWith(AssignDevicesToRoomDialogComponent, expect.objectContaining({
      data: room
    }));
  })
})
