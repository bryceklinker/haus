import {createFeatureSelector} from "@ngrx/store";
import {getSelectors, RouterReducerState} from "@ngrx/router-store";
import {RouterState} from "../../../app.state";

const selectRouterState = createFeatureSelector<RouterReducerState<RouterState>>('router');
export const {
  selectRouteParam,
  selectCurrentRoute,
  selectFragment,
  selectQueryParam,
  selectRouteData,
  selectQueryParams,
  selectRouteParams,
  selectUrl
} = getSelectors(selectRouterState);
