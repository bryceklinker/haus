import {createAction, INIT, props} from "@ngrx/store";
import {RouterState} from "../app/app.state";

export const initAction = createAction(INIT);

export const setRouterState = createAction('[TESTING] SET ROUTER STATE', (state: Partial<RouterState>) => ({payload: state}));

export const TestingActions = {
  initAction,
  setRouterState
}
