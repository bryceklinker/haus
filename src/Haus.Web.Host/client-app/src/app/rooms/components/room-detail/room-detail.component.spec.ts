import {RoomDetailComponent} from './room-detail.component';
import {ModelFactory, TestingEventEmitter} from "../../../../testing";
import {DeviceType, LightingState, RoomLightingChangedEvent, RoomModel} from "../../../shared/models";
import {RoomDetailHarness} from "./room-detail.harness";

describe('RoomDetailComponent', () => {
  it('should show room name', async () => {
    const room = ModelFactory.createRoomModel({name: 'new hotness'});

    const harness = await RoomDetailHarness.render({room});

    expect(harness.roomDetail).toHaveTextContent('new hotness');
  })

  it('should show each device', async () => {
    const devices = [
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel()
    ];

    const harness = await RoomDetailHarness.render({devices});

    expect(harness.devices).toHaveLength(2);
  })

  it('should show each device\'s info', async () => {
    const devices = [
      ModelFactory.createDeviceModel({name: 'bob', deviceType: DeviceType.Light})
    ];

    const harness = await RoomDetailHarness.render({devices});

    expect(harness.container).toHaveTextContent(/bob/g);
    expect(harness.container).toHaveTextContent(/Light/g);
  })

  it('should show room lighting', async () => {
    const room = ModelFactory.createRoomModel();

    const harness = await RoomDetailHarness.render({room});

    expect(harness.lighting).toBeInTheDocument();
  })

  it('should notify when lighting changed', async () => {
    const room = ModelFactory.createRoomModel({
      lighting: ModelFactory.createLighting({state: LightingState.Off})
    });
    const emitter = new TestingEventEmitter<RoomLightingChangedEvent>();

    const harness = await RoomDetailHarness.render({room, lightingChange: emitter});
    await harness.turnRoomOn();

    expect(emitter.emit).toHaveBeenCalledWith({
      room: room,
      lighting: expect.objectContaining({state: LightingState.On})
    })
  })

  it('should notify when assigning devices to room', async () => {
    const room = ModelFactory.createRoomModel();
    const emitter = new TestingEventEmitter<RoomModel>();

    const harness = await RoomDetailHarness.render({room, assignDevices: emitter});
    await harness.assignDevices();

    expect(emitter.emit).toHaveBeenCalledWith(room);
  })
});
