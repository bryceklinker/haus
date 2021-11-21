import {createAction, Creator, NotAllowedCheck} from "@ngrx/store";

export function createAsyncActionSet<
  TRequestParams extends any[], TRequest extends object,
  TSuccessParams extends any[], TSuccess extends object,
  TFailedParams extends any[], TFailed extends object>(
  type: string,
  requestConfig: Creator<TRequestParams, TRequest & NotAllowedCheck<TRequest>>,
  successConfig: Creator<TSuccessParams, TSuccess & NotAllowedCheck<TSuccess>>,
  failedConfig: Creator<TFailedParams, TFailed & NotAllowedCheck<TFailed>>
) {
  return {
    request: createAction(`${type} Request`, requestConfig),
    success: createAction(`${type} Success`, successConfig),
    failed: createAction(`${type} Failed`, failedConfig)
  }
}
