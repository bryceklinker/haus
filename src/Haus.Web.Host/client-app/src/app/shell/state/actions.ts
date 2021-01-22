import {createAsyncActionSet} from "../../shared/actions";
import {ApplicationPackageModel, ApplicationVersionModel, ListResult} from "../../shared/models";

export const ShellActions = {
  loadLatestVersion: createAsyncActionSet(
    '[Shell] Load Latest Version',
    () => ({payload: 'ignore'}),
    (model: ApplicationVersionModel) => ({payload: model}),
    (error: any) => ({payload: error})
  ),

  loadLatestPackages: createAsyncActionSet(
    '[Shell] Load Latest Packages',
    () => ({payload: 'ignore'}),
    (model: ListResult<ApplicationPackageModel>) => ({payload: model.items}),
    (error: any) => ({payload: error})
  ),

  downloadPackage: createAsyncActionSet(
    '[Shell] Download Package',
    (model: ApplicationPackageModel) => ({payload: model}),
    () => ({payload: 'ignore'}),
    (error: any) => ({payload: error})
  )
};
