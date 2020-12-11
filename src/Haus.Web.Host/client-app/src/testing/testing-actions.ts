import {createAction, INIT, props} from "@ngrx/store";
import {RouterUrlState} from "../app/app.state";

const initAction = createAction(INIT);
const setRouterState = createAction('[Testing] Set Router State', (state: Partial<RouterState>) => ({payload: state}))
export const TestingActions = {
  initAction,
  setRouterState
}
