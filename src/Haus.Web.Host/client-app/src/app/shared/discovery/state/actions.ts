import {createAsyncActionSet} from "../../actions";
import {DiscoveryModel} from "../../models";

export const DiscoveryActions = {
  startDiscovery: createAsyncActionSet(
    '[Discovery] Start',
    () => ({payload: 'ignore'}),
    () => ({payload: 'ignore'}),
    (err: any) => ({payload: err})
  ),
  stopDiscovery: createAsyncActionSet(
    '[Discovery] Stop',
    () => ({payload: 'ignore'}),
    () => ({payload: 'ignore'}),
    (err: any) => ({payload: err})
  ),
  syncDiscovery: createAsyncActionSet(
    '[Discovery] Sync',
    () => ({payload: 'ignore'}),
    () => ({payload: 'ignore'}),
    (err: any) => ({payload: err})
  ),
  getDiscovery: createAsyncActionSet(
    '[Discovery] Get',
    () => ({payload: 'ignore'}),
    (payload: DiscoveryModel) => ({payload}),
    (err: any) => ({payload: err})
  )
}
