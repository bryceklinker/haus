import {createAction, INIT} from "@ngrx/store";
import {RouterUrlState} from "../app/shared/routing";
import {EntityActionFactory, EntityOp} from "@ngrx/data";

const initAction = createAction(INIT);
const setRouterState = createAction('[Testing] Set Router State', (state: Partial<RouterUrlState>) => ({payload: state}))

const createQueryAllSuccess = <TData>(entityName: string, data?: TData) => new EntityActionFactory().create<TData>(entityName, EntityOp.QUERY_ALL_SUCCESS, data);
const createQueryAll = (entityName: string, data?: any) => new EntityActionFactory().create(entityName, EntityOp.QUERY_ALL, data || null);
export const TestingActions = {
  initAction,
  setRouterState,
  createQueryAll,
  createQueryAllSuccess
}
