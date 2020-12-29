import {devicesReducer} from "./devices.reducer";
import {DevicesActions} from "./actions";
import {ModelFactory} from "../../../testing";
import {generateStateFromActions} from "../../../testing/app-state-generator";

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
})
