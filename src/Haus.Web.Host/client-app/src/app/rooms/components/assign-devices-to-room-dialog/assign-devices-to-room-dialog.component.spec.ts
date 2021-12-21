import {eventually, ModelFactory} from "../../../../testing";
import {AssignDevicesToRoomDialogComponent} from "./assign-devices-to-room-dialog.component";
import {DevicesActions} from "../../../devices/state";
import {RoomsActions} from "../../state";
import {DeviceModel, RoomModel} from "../../../shared/models";
import {AssignDevicesToRoomDialogHarness} from "./assign-devices-to-room-dialog.harness";

describe('AssignDevicesToRoomDialogComponent', () => {
  let unassignedDevices: Array<DeviceModel>;
  let room: RoomModel;
  let harness: AssignDevicesToRoomDialogHarness;

  beforeEach(async () => {
    room = ModelFactory.createRoomModel();
    unassignedDevices = [
      ModelFactory.createDeviceModel({roomId: undefined, name: 'one'}),
      ModelFactory.createDeviceModel({roomId: undefined, name: 'two'}),
      ModelFactory.createDeviceModel({roomId: undefined, name: 'three'}),
    ]
    const devices = [
      ModelFactory.createDeviceModel({roomId: 1}),
      ...unassignedDevices,
      ModelFactory.createDeviceModel({roomId: 4}),
    ];

    harness = await AssignDevicesToRoomDialogHarness.render(room,
      DevicesActions.loadDevices.success(ModelFactory.createListResult(...devices))
    );
  })

  test('should show all unassigned devices', () => {
    expect(harness.unassignedDevices).toHaveLength(3);
  })

  test('should disallow assignment when no devices have been selected', () => {
    expect(harness.assignDevicesElement).toBeDisabled();
  })

  test('should request to assign devices to room when assign triggered', async () => {
    await harness.selectDevice(unassignedDevices[0]);

    await harness.assignDevices();

    expect(harness.dispatchedActions).toContainEqual(RoomsActions.assignDevicesToRoom.request({
      roomId: room.id,
      deviceIds: [unassignedDevices[0].id]
    }))
  })

  test('should only add devices that have been selected when assigning devices is triggered', async () => {
    await harness.selectDevice(unassignedDevices[1]);
    await harness.selectDevice(unassignedDevices[0]);
    await harness.selectDevice(unassignedDevices[1]);
    await harness.selectDevice(unassignedDevices[2]);

    await harness.assignDevices();

    expect(harness.dispatchedActions).toContainEqual(RoomsActions.assignDevicesToRoom.request({
      roomId: room.id,
      deviceIds: [unassignedDevices[0].id, unassignedDevices[2].id]
    }))
  })

  test('should close dialog when assignment is successful', async () => {
    harness.simulateAssignDevicesSuccess(54);

    await eventually(() => {
      expect(harness.dialogRef.close).toHaveBeenCalled();
    })
  })

  test('should close dialog when assignment is cancelled', async () => {
    await harness.cancel()

    expect(harness.dialogRef.close).toHaveBeenCalled();
  })
})
