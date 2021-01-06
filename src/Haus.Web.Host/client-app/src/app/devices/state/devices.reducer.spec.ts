import {devicesReducer} from "./devices.reducer";
import {DevicesActions} from "./actions";
import {ModelFactory} from "../../../testing";
import {generateStateFromActions} from "../../../testing/app-state-generator";
import {EventsActions} from "../../shared/events";
import {RoomsActions} from "../../rooms/state";

describe('devicesReducer', () => {
  it('should add all devices to state when load devices finishes', () => {
    const devices = ModelFactory.createListResult(
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel(),
    )
    const state = generateStateFromActions(devicesReducer, DevicesActions.loadDevices.success(devices));

    expect(state.ids).toHaveLength(3);
    expect(state.entities[devices.items[0].id]).toEqual(devices.items[0]);
    expect(state.entities[devices.items[1].id]).toEqual(devices.items[1]);
    expect(state.entities[devices.items[2].id]).toEqual(devices.items[2]);
  })

  it('should add device when device created event received', () => {
    const device = ModelFactory.createDeviceModel();

    const state = generateStateFromActions(devicesReducer,
      EventsActions.deviceCreated({device})
    );

    expect(state.entities[device.id]).toEqual(device);
  })

  it('should update device when device updated event received', () => {
    const updated = ModelFactory.createDeviceModel({id: 7});

    const state = generateStateFromActions(devicesReducer,
      EventsActions.deviceCreated({device: ModelFactory.createDeviceModel({id: 7})}),
      EventsActions.deviceUpdated({device: updated})
    );

    expect(state.entities[7]).toEqual(updated);
  })

  it('should update room id for devices when devices assigned to room is successful', () => {
    const device = ModelFactory.createDeviceModel();

    const state = generateStateFromActions(devicesReducer,
      EventsActions.deviceCreated({device}),
      RoomsActions.assignDevicesToRoom.success({roomId: 66, deviceIds: [device.id]})
    );

    expect(state.entities[device.id]?.roomId).toEqual(66);
  })

  it('should update room id for devices when devices assigned to room event received', () => {
    const existing = ModelFactory.createDeviceModel();

    const state = generateStateFromActions(devicesReducer,
      EventsActions.deviceCreated({device: existing}),
      EventsActions.devicesAssignedToRoom({roomId: 76, deviceIds: [existing.id]})
    );

    expect(state.entities[existing.id]?.roomId).toEqual(76);
  })

  it('should update device lighting when device lighting changed event received', () => {
    const original = ModelFactory.createDeviceModel();
    const lighting = ModelFactory.createLighting({level: 98});

    const state = generateStateFromActions(devicesReducer,
      EventsActions.deviceCreated({device: original}),
      EventsActions.deviceLightingChanged({device: original, lighting})
    );

    expect(state.entities[original.id]?.lighting).toEqual(lighting);
  })
})
