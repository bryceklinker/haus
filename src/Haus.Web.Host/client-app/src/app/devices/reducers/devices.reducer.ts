import {Action, createFeatureSelector, createReducer, createSelector, on} from "@ngrx/store";
import {selectRouteParam} from '../../shared/routing';
import {DevicesState} from "../devices.state";
import {DevicesActions} from "../actions";

export const DEVICES_FEATURE_KEY = 'devices';

const INITIAL_STATE: DevicesState = {
  devices: {},
  allowDiscovery: false
};

const reducer = createReducer(INITIAL_STATE,
  on(DevicesActions.load.success, (state, {payload}) => ({
    ...state,
    devices: {
      ...payload.items.reduce((s, model) => ({...s, [model.id]: model}), state.devices)
    }
  })),
  on(DevicesActions.startDiscovery.success, (state) => ({...state, allowDiscovery: true})),
  on(DevicesActions.stopDiscovery.success, (state) => ({...state, allowDiscovery: false}))
);

export function devicesReducer(state: DevicesState | undefined, action: Action): DevicesState {
  return reducer(state, action);
}

const selectDevicesState = createFeatureSelector<DevicesState>(DEVICES_FEATURE_KEY);
export const selectDevicesById = createSelector(selectDevicesState, s => s.devices);
export const selectDevices = createSelector(selectDevicesState, s => Object.keys(s.devices).map(key => s.devices[Number(key)]))
export const selectDevicesAllowDiscovery = createSelector(selectDevicesState, s => s.allowDiscovery);
export const selectDeviceById = createSelector(
  selectDevicesById,
  selectRouteParam('deviceId'),
  (devicesById, deviceId) => {
    return deviceId ? devicesById[deviceId] : null
  }
)
