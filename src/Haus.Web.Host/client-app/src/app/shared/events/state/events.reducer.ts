import {EventsState} from "./events.state";
import {HausEvent} from "../../models";
import {Action, createSelector} from "@ngrx/store";
import {sortArrayBy, SortDirection} from "../../sort-array-by";
import {AppState} from "../../../app.state";

const initialState: EventsState = {
  events: []
};

export function eventsReducer(state: EventsState | undefined = initialState, action: Action): EventsState {
  if (!state) {
    return initialState;
  }

  const hausEvent = action as HausEvent
  if (hausEvent.isEvent) {
    return {
      ...state,
      events: sortArrayBy([
        ...state.events.slice(0, 99),
        hausEvent
      ], e => e.timestamp, SortDirection.Descending)
    }
  }
  return state;
}

const selectEventsState = (state: AppState) => state.events;
export const selectAllEvents = createSelector(selectEventsState, s => s.events);
