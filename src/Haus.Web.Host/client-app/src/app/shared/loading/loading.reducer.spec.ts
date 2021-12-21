import {loadingReducer, selectIsLoading} from "./loading.reducer";
import {generateAppStateFromActions, generateStateFromActions} from "../../../testing/app-state-generator";

describe('loadingReducer', () => {
  test('should be loading when request action received', () => {
    const state = generateStateFromActions(loadingReducer,
      {type: 'something Request'});

    expect(state['something']).toEqual(true);
  })

  test('should stop loading when success action received', () => {
    const state = generateStateFromActions(loadingReducer,
      {type: 'something Request'},
      {type: 'something Success'});

    expect(state['something']).toEqual(false);
  })

  test('should stop loading when failed action received', () => {
    const state = generateStateFromActions(loadingReducer,
      {type: 'something Request'},
      {type: 'something Failed'});

    expect(state['something']).toEqual(false);
  })

  test('should ignore non async actions', () => {
    const state = generateStateFromActions(loadingReducer,
      {type: 'idk'});

    expect(state).toEqual({});
  })

  test('selecting loading state should return false by default', () => {
    const state = generateAppStateFromActions();

    expect(selectIsLoading({type: 'something'})(state)).toEqual(false);
  })

  test('selecting loading state is selected for type that has not been seen should return false by default', () => {
    const state = generateAppStateFromActions();

    expect(selectIsLoading({type: 'something Request'})(state)).toEqual(false);
  })
})
