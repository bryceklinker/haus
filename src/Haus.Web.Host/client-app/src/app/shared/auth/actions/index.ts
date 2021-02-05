import {createAction} from "@ngrx/store";
import {UserModel} from "../user.model";

export const AuthActions = {
  userLoggedIn: createAction('[Auth] User Logged In', (model: UserModel) => ({payload: model})),
  userLoggedOut: createAction('[Auth] User Logged Out'),
  logout: createAction('[Auth] Log Out'),
  isLoading: createAction('[Auth] Is Loading', (isLoading: boolean) => ({payload: isLoading}))
}
