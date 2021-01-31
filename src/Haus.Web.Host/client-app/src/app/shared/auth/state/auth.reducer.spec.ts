import {generateStateFromActions} from "../../../../testing/app-state-generator";
import {authReducer} from "./auth.reducer";
import {AuthActions} from "../actions";
import {ModelFactory} from "../../../../testing";

describe('authReducer', () => {
  it('should not have a user when initialized', () => {
    const state = generateStateFromActions(authReducer)

    expect(state.user).toBeNull();
  })

  it('should have user when user logged in', () => {
    const user = ModelFactory.createUser();

    const state = generateStateFromActions(authReducer,
      AuthActions.userLoggedIn(user)
    );

    expect(state.user).toEqual(user);
  })

  it('should not have user when user logged out', () => {
    const state = generateStateFromActions(authReducer,
      AuthActions.userLoggedIn(ModelFactory.createUser()),
      AuthActions.userLoggedOut()
    );

    expect(state.user).toBeNull();
  })
})
