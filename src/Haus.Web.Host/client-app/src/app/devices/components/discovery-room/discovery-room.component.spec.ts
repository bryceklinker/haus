import {DiscoveryRoomComponent} from "./discovery-room.component";
import {ModelFactory, TestingEventEmitter} from "../../../../testing";
import {DevicesAssignedToRoomEvent} from "../../../shared/models";
import {DiscoveryRoomHarness} from "./discovery-room.harness";

describe('DiscoveryRoomComponent', () => {
  it('should show room name', async () => {
    const room = ModelFactory.createRoomModel();

    const harness = await DiscoveryRoomHarness.render({room});

    expect(harness.container).toHaveTextContent(room.name);
  })

  it('should set the id of drop zone', async () => {
    const room = ModelFactory.createRoomModel();

    const harness = await DiscoveryRoomHarness.render({room});

    expect(harness.room).toBeInTheDocument();
  })

  it('should show devices in room', async () => {
    const room = ModelFactory.createRoomModel();
    const devices = [
      ModelFactory.createDeviceModel({roomId: room.id}),
      ModelFactory.createDeviceModel({roomId: undefined}),
      ModelFactory.createDeviceModel({roomId: room.id}),
    ];

    const harness = await DiscoveryRoomHarness.render({room, devices});

    expect(harness.container).toHaveTextContent(devices[0].name);
    expect(harness.container).toHaveTextContent(devices[2].name);
    expect(harness.container).not.toHaveTextContent(devices[1].name);
  })

  it('should notify when device is assigned to room', async () => {
    const room = ModelFactory.createRoomModel();
    const device = ModelFactory.createDeviceModel();
    const eventEmitter = new TestingEventEmitter<DevicesAssignedToRoomEvent>()

    const harness = await DiscoveryRoomHarness.render({room, assignDevice: eventEmitter});
    await harness.dropDeviceOnRoom(device);

    expect(eventEmitter.emit).toHaveBeenCalledWith({roomId: room.id, deviceIds: [device.id]});
  })
})
