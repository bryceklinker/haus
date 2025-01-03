import {ModelFactory} from "../../../../testing";
import {DiscoveryRootComponent} from "./discovery-root.component";
import {DiscoveryActions} from "../../../shared/discovery";
import {EventsActions} from "../../../shared/events";
import {RoomsActions} from "../../../rooms/state";
import {DevicesActions} from "../../state";
import {DiscoveryRootHarness} from "./discovery-root.harness";

describe('DiscoveryRootComponent', () => {
  test('should start discovery when rendered', async () => {
    const harness = await DiscoveryRootHarness.render();

    expect(harness.dispatchedActions).toContainEqual(DiscoveryActions.startDiscovery.request());
  })

  test('should load rooms when rendered', async () => {
    const harness = await DiscoveryRootHarness.render();

    expect(harness.dispatchedActions).toContainEqual(RoomsActions.loadRooms.request());
  })

  test('should load devices when rendered', async () => {
    const harness = await DiscoveryRootHarness.render();

    expect(harness.dispatchedActions).toContainEqual(DevicesActions.loadDevices.request());
  })

  test('should stop discovery when destroyed', async () => {
    const harness = await DiscoveryRootHarness.render();

    harness.destroy();

    expect(harness.dispatchedActions).toContainEqual(DiscoveryActions.stopDiscovery.request());
  })

  test('should show unassigned devices', async () => {
    const harness = await DiscoveryRootHarness.render(
      EventsActions.deviceCreated({device: ModelFactory.createDeviceModel({roomId: undefined})}),
      EventsActions.deviceCreated({device: ModelFactory.createDeviceModel({roomId: 1})}),
      EventsActions.deviceCreated({device: ModelFactory.createDeviceModel({roomId: 4})}),
      EventsActions.deviceCreated({device: ModelFactory.createDeviceModel({roomId: undefined})})
    );

    expect(harness.unassignedDevices).toHaveLength(2);
  })

  test('should show all rooms', async () => {
    const harness = await DiscoveryRootHarness.render(
      EventsActions.roomCreated({room: ModelFactory.createRoomModel()}),
      EventsActions.roomCreated({room: ModelFactory.createRoomModel()}),
      EventsActions.roomCreated({room: ModelFactory.createRoomModel()}),
    );

    expect(harness.rooms).toHaveLength(3);
  })

  test('should show assigned devices', async () => {
    const room = ModelFactory.createRoomModel();
    const device = ModelFactory.createDeviceModel({roomId: room.id});
    const harness = await DiscoveryRootHarness.render(
      EventsActions.roomCreated({room}),
      EventsActions.deviceCreated({device})
    );

    expect(harness.unassignedDevices).toHaveLength(0);
    expect(harness.assignedDevices).toHaveLength(1);
  })

  test('should notify when device is assigned to room', async () => {
    const room = ModelFactory.createRoomModel();
    const device = ModelFactory.createDeviceModel();
    const harness = await DiscoveryRootHarness.render(
      EventsActions.roomCreated({room}),
      EventsActions.deviceCreated({device})
    );

    await harness.assignDevice(device);

    expect(harness.dispatchedActions).toContainEqual(RoomsActions.assignDevicesToRoom.request({roomId: room.id, deviceIds: [device.id]}))
  })
})
