import {DevicesState} from "../devices.state";
import {Action, createReducer, on} from "@ngrx/store";
import {DevicesActions} from "../actions";

export const DEVICES_FEATURE_KEY = 'devices';

const INITIAL_STATE: DevicesState = {};

const reducer = createReducer(INITIAL_STATE,
  on(DevicesActions.load.success, (state, {payload}) => ({
    ...state,
    ...payload.items.reduce((s, model) => ({...s, [model.id]: model}), state)
  }))
);

export function devicesReducer(state: DevicesState | undefined, action: Action): DevicesState {
  return reducer(state, action);
}
