import {createAction, INIT} from "@ngrx/store";
import {RouterUrlState} from "../app/shared/routing";
import {EntityActionFactory, EntityOp} from "@ngrx/data";

const initAction = createAction(INIT);
const setRouterState = createAction('[Testing] Set Router State', (state: Partial<RouterUrlState>) => ({payload: state}))

const createQueryAllSuccess = <TData>(entityName: string, data?: TData) => new EntityActionFactory().create<TData>(entityName, EntityOp.QUERY_ALL_SUCCESS, data);
export const TestingActions = {
  initAction,
  setRouterState,
  createQueryAllSuccess
}
