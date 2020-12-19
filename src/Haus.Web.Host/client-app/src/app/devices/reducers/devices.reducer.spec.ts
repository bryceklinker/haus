import {runActionsThroughReducer} from "../../../testing";
import {devicesReducer} from "./devices.reducer";
import {DevicesActions} from "../actions";

describe('devicesReducer', () => {
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
