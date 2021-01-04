import {devicesReducer} from "./devices.reducer";
import {DevicesActions} from "./actions";
import {ModelFactory} from "../../../testing";
import {generateStateFromActions} from "../../../testing/app-state-generator";
import {EventsActions} from "../../shared/events";

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

  it('should allow discovery of devices', () => {
    const state = generateStateFromActions(devicesReducer, DevicesActions.startDiscovery.success());

    expect(state.allowDiscovery).toEqual(true);
  })

  it('should disable discovery of devices', () => {
    const state = generateStateFromActions(devicesReducer,
      DevicesActions.startDiscovery.success(),
      DevicesActions.stopDiscovery.success());

    expect(state.allowDiscovery).toEqual(false);
  })

  it('should allow discovery of devices when discovery started event received', () => {
    const state = generateStateFromActions(devicesReducer,
      EventsActions.discoveryStarted({})
    );

    expect(state.allowDiscovery).toEqual(true);
  })

  it('should disable discovery of devices when discovery stopped', () => {
    const state = generateStateFromActions(devicesReducer,
      EventsActions.discoveryStarted({}),
      EventsActions.discoveryStopped({})
    );

    expect(state.allowDiscovery).toEqual(false);
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

  it('should update room id for devices when devices assigned to room event received', () => {
    const existing = ModelFactory.createDeviceModel();

    const state = generateStateFromActions(devicesReducer,
      EventsActions.deviceCreated({device: existing}),
      EventsActions.devicesAssignedToRoom({roomId: 76, deviceIds: [existing.id]})
    );

    expect(state.entities[existing.id]?.roomId).toEqual(76);
  })
})
