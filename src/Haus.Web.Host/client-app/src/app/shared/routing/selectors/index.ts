import {createFeatureSelector, createSelector} from "@ngrx/store";
import {RouterReducerState} from "@ngrx/router-store";
import {RouterUrlState} from "../router-url.state";

const selectRouterState = createFeatureSelector<RouterReducerState<RouterUrlState>>('router');
export const selectRouteParam = (paramName: string) => createSelector(
  selectRouterState,
  (routerState: RouterReducerState<RouterUrlState>) => {
    return routerState.state.params[paramName]
  })