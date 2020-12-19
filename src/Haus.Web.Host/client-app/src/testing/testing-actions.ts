import {createAction, INIT} from "@ngrx/store";
import {RouterUrlState} from "../app/shared/routing";
import {EntityActionFactory, EntityOp} from "@ngrx/data";

const initAction = createAction(INIT);
const setRouterState = createAction('[Testing] Set Router State', (state: Partial<RouterUrlState>) => ({payload: state}))

const actionFactory = new EntityActionFactory();
const createQueryAllSuccess = (entityName: string, data?: any) => actionFactory.create(entityName, EntityOp.QUERY_ALL_SUCCESS, data);
const createQueryAll = (entityName: string, data?: any) => actionFactory.create(entityName, EntityOp.QUERY_ALL, data || null);
const addOne = (entityName: string, data?: any) => actionFactory.create(entityName, EntityOp.SAVE_ADD_ONE, data || null);
export const TestingActions = {
  initAction,
  setRouterState,
  createQueryAll,
  createQueryAllSuccess,
  addOne
}
