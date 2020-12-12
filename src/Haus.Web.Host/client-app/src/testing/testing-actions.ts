import {createAction, INIT} from "@ngrx/store";
import {RouterUrlState} from "../app/shared/routing";

const initAction = createAction(INIT);
const setRouterState = createAction('[Testing] Set Router State', (state: Partial<RouterUrlState>) => ({payload: state}))
export const TestingActions = {
  initAction,
  setRouterState
}
