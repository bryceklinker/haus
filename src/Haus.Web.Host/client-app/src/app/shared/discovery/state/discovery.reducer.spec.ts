import {generateStateFromActions} from "../../../../testing/app-state-generator";
import {discoveryReducer} from "./discovery.reducer";
import {DiscoveryActions} from "./actions";
import {EventsActions} from "../../events";
import {DiscoveryState} from "../../models";

describe('discoveryReducer', () => {
  it('should allow discovery when discovery started successfully', () => {
    const state = generateStateFromActions(discoveryReducer,
      DiscoveryActions.startDiscovery.success()
    );

    expect(state.isDiscoveryAllowed).toEqual(true);
  })

  it('should not allow discovery when discovery stopped successfully', () => {
    const state = generateStateFromActions(discoveryReducer,
      DiscoveryActions.startDiscovery.success(),
      DiscoveryActions.stopDiscovery.success()
    );

    expect(state.isDiscoveryAllowed).toEqual(false);
  })

  it('should allow discovery when discovery started event received', () => {
    const state = generateStateFromActions(discoveryReducer,
      EventsActions.discoveryStarted({})
    );

    expect(state.isDiscoveryAllowed).toEqual(true);
  })

  it('should not allow discovery when discovery stopped event received', () => {
    const state = generateStateFromActions(discoveryReducer,
      EventsActions.discoveryStopped({})
    );

    expect(state.isDiscoveryAllowed).toEqual(false);
  })

  it('should allow discovery when get discovery successful has enabled state', () => {
    const state = generateStateFromActions(discoveryReducer,
      DiscoveryActions.getDiscovery.success({state: DiscoveryState.Enabled})
    );

    expect(state.isDiscoveryAllowed).toEqual(true);
  })

  it('should not allow discovery when get discovery successful has disabled state', () => {
    const state = generateStateFromActions(discoveryReducer,
      DiscoveryActions.getDiscovery.success({state: DiscoveryState.Enabled}),
      DiscoveryActions.getDiscovery.success({state: DiscoveryState.Disabled}),
    );

    expect(state.isDiscoveryAllowed).toEqual(false);
  })
})
