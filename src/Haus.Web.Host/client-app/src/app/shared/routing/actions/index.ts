import {createAction} from "@ngrx/store";

export const RouterActions = {
  navigate: createAction('[Router] Navigate', (url: string) => ({payload: url}))
};
