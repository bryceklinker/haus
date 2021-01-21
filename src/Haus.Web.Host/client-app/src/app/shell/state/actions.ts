import {createAsyncActionSet} from "../../shared/actions";
import {ApplicationVersionModel} from "../../shared/models";

export const ShellActions = {
  loadLatestVersion: createAsyncActionSet(
    '[Shell] Load Latest Version',
    () => ({payload: 'ignore'}),
    (model: ApplicationVersionModel) => ({payload: model}),
    (error: any) => ({payload: error})
  )
};
