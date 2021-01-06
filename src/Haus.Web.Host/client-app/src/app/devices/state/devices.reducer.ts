import {createEntityAdapter} from "@ngrx/entity";
import {Action, createReducer, createSelector, on} from "@ngrx/store";
import {createComparer} from "../../shared/sort-array-by";
import {DevicesState} from "./devices.state";
import {DevicesActions} from "./actions";
import {AppState} from "../../app.state";
import {DeviceModel} from "../../shared/models";
import {EventsActions} from "../../shared/events";
import {RoomsActions} from "../../rooms/state";

const adapter = createEntityAdapter<DeviceModel>({
  selectId: device => device.id,
  sortComparer: createComparer<DeviceModel>(d => d.name)
});

const initialState: DevicesState = adapter.getInitialState({
  allowDiscovery: false
});
const reducer = createReducer(initialState,
  on(DevicesActions.loadDevices.success, (state, {payload}) => adapter.upsertMany(payload.items, state)),
  on(DevicesActions.startDiscovery.success, EventsActions.discoveryStarted, (state) => ({
    ...state,
    allowDiscovery: true
  })),
  on(DevicesActions.stopDiscovery.success, EventsActions.discoveryStopped, (state) => ({
    ...state,
    allowDiscovery: false
  })),
  on(EventsActions.deviceCreated, EventsActions.deviceUpdated, (state, {payload}) => adapter.upsertOne(payload.device, state)),
  on(RoomsActions.assignDevicesToRoom.success, EventsActions.devicesAssignedToRoom, (state, {payload}) => adapter.updateMany(
    payload.deviceIds.map(id => ({id, changes: {roomId: payload.roomId}})),
    state
    )
  ),
  on(EventsActions.deviceLightingChanged, (state, {payload}) => ({
    ...adapter.updateOne({
      id: payload.device.id,
      changes: {lighting: payload.lighting}
    }, state)
  }))
);

export function devicesReducer(state: DevicesState | undefined, action: Action) {
  return reducer(state, action);
}

const {
  selectAll,
  selectEntities
} = adapter.getSelectors();
const selectDevicesState = (state: AppState) => state.devices;
export const selectAllDevices = createSelector(selectDevicesState, selectAll);
export const selectDeviceById = (id: string | null) => createSelector(
  selectDevicesState,
  (state) => id ? selectEntities(state)[id] : null);
export const selectAllowDevicesToBeDiscovered = createSelector(selectDevicesState, s => s.allowDiscovery);
export const selectAllDevicesByRoomId = (roomId: string | null) =>
  createSelector(selectDevicesState,
    (state) => roomId ? selectAll(state).filter(d => d.roomId === Number(roomId)) : [])
export const selectUnassignedDevices = createSelector(selectAllDevices, s => s.filter(d => !d.roomId))
