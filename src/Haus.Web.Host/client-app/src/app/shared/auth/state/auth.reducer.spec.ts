import {generateStateFromActions} from "../../../../testing/app-state-generator";
import {authReducer} from "./auth.reducer";
import {AuthActions} from "../actions";
import {ModelFactory} from "../../../../testing";

describe('authReducer', () => {
  test('should not have a user when initialized', () => {
    const state = generateStateFromActions(authReducer)

    expect(state.user).toBeNull();
    expect(state.isLoading).toEqual(true);
  })

  test('should have user when user logged in', () => {
    const user = ModelFactory.createUser();

    const state = generateStateFromActions(authReducer,
      AuthActions.userLoggedIn(user)
    );

    expect(state.user).toEqual(user);
  })

  test('should not have user when user logged out', () => {
    const state = generateStateFromActions(authReducer,
      AuthActions.userLoggedIn(ModelFactory.createUser()),
      AuthActions.userLoggedOut()
    );

    expect(state.user).toBeNull();
  })

  test('should be done loading when auth is loading false received', () => {
    const state = generateStateFromActions(authReducer,
      AuthActions.isLoading(false)
    );

    expect(state.isLoading).toEqual(false);
  })
})
