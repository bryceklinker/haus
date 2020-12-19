import {Action, createFeatureSelector, createReducer, createSelector, on} from "@ngrx/store";
import {EntitySelectorsFactory} from "@ngrx/data";

import {selectRouteParam} from '../../shared/routing';
import {DevicesState} from "../devices.state";
import {DevicesActions} from "../actions";
import {DeviceModel} from "../models";
import {ENTITY_NAMES} from "../../entity-metadata";

export const DEVICES_FEATURE_KEY = 'devices';

const INITIAL_STATE: DevicesState = {
  allowDiscovery: false
};

const reducer = createReducer(INITIAL_STATE,
  on(DevicesActions.startDiscovery.success, (state) => ({...state, allowDiscovery: true})),
  on(DevicesActions.stopDiscovery.success, (state) => ({...state, allowDiscovery: false}))
);

export function devicesReducer(state: DevicesState | undefined, action: Action): DevicesState {
  return reducer(state, action);
}

const deviceSelectors = new EntitySelectorsFactory().create<DeviceModel>(ENTITY_NAMES.Device);

const selectDevicesState = createFeatureSelector<DevicesState>(DEVICES_FEATURE_KEY);
export const selectDevicesAllowDiscovery = createSelector(selectDevicesState, s => s.allowDiscovery);
export const selectDeviceById = createSelector(
  deviceSelectors.selectEntityMap,
  selectRouteParam('deviceId'),
  (devicesById, deviceId) => {
    return deviceId ? devicesById[deviceId] : null
  }
)
