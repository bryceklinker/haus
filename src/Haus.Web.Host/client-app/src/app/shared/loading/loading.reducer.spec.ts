import {runActionsThroughReducer} from "../../../testing";
import {loadingReducer} from "./loading.reducer";

describe('loadingReducer', () => {
  it('should be loading when request action received', () => {
    const state = runActionsThroughReducer(loadingReducer,
      {type: 'something Request'});

    expect(state['something']).toEqual(true);
  })

  it('should stop loading when success action received', () => {
    const state = runActionsThroughReducer(loadingReducer,
      {type: 'something Request'},
      {type: 'something Success'});

    expect(state['something']).toEqual(false);
  })

  it('should stop loading when failed action received', () => {
    const state = runActionsThroughReducer(loadingReducer,
      {type: 'something Request'},
      {type: 'something Failed'});

    expect(state['something']).toEqual(false);
  })

  it('should ignore non async actions', () => {
    const state = runActionsThroughReducer(loadingReducer,
      {type: 'idk'});

    expect(state).toEqual({});
  })
})
