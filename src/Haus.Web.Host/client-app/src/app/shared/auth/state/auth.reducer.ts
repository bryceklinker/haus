import {Action, createReducer, createSelector, on} from "@ngrx/store";
import {AuthState} from "./auth.state";
import {AuthActions} from "../actions";
import {AppState} from "../../../app.state";

const initialState: AuthState = {
  user: null,
  isLoading: true,
};

const reducer = createReducer(initialState,
  on(AuthActions.userLoggedIn, (state, {payload}) => ({...state, user: payload})),
  on(AuthActions.userLoggedOut, (state) => ({...state, user: null})),
  on(AuthActions.isLoading, (state, {payload}) => ({...state, isLoading: payload}))
);

export function authReducer(state: AuthState | undefined, action: Action): AuthState {
  return reducer(state, action);
}

const selectAuthState = (state: AppState) => state.auth;
export const selectUser = createSelector(selectAuthState, s => s.user);
export const selectIsAuthLoading = createSelector(selectAuthState, s => s.isLoading);
