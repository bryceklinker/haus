import {runActionsThroughReducer} from "../../../testing";
import {devicesReducer} from "./devices.reducer";
import {DevicesActions} from "../actions";
import {ModelFactory} from "../../../testing/model-factory";

describe('devicesReducer', () => {
  it('should add device to hash of devices', () => {
    const deviceOne = ModelFactory.createDeviceModel({id: 5});
    const deviceTwo = ModelFactory.createDeviceModel({id: 2});
    const deviceThree = ModelFactory.createDeviceModel({id: 8});
    const result = ModelFactory.createListResult(
      deviceOne,
      deviceTwo,
      deviceThree,
    )

    const state = runActionsThroughReducer(devicesReducer,
      DevicesActions.load.success(result));

    expect(state.devices[5]).toEqual(deviceOne);
    expect(state.devices[2]).toEqual(deviceTwo);
    expect(state.devices[8]).toEqual(deviceThree);
  })

  it('should allow discovery when start discovery is successful', () => {
    const state = runActionsThroughReducer(devicesReducer,
      DevicesActions.startDiscovery.success()
    );

    expect(state.allowDiscovery).toEqual(true);
  })

  it('should disallow discovery when stop discovery is successful', () => {
    const state = runActionsThroughReducer(devicesReducer,
      DevicesActions.startDiscovery.success(),
      DevicesActions.stopDiscovery.success()
    );

    expect(state.allowDiscovery).toEqual(false);
  })
})
