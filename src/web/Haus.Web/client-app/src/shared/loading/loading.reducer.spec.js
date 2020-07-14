import {initAction} from '../../testing/testing.actions';
import {loadingReducer, selectIsLoading} from './loading.reducer';
import {createAsyncActions} from '../state/actions';

const testLoadingActions = createAsyncActions('loading');

test('when initialized then initial loading state returned', () => {
    const state = loadingReducer(undefined, initAction());

    expect(state).toEqual({});
});

test('when request action received then request is loading', () => {
    let state = loadingReducer(undefined, initAction());
    state = loadingReducer(state, testLoadingActions.request());

    expect(state).toEqual({loading: true});
});

test('when success action received then request is not loading', () => {
    let state = loadingReducer(undefined, initAction());
    state = loadingReducer(state, testLoadingActions.request());
    state = loadingReducer(state, testLoadingActions.success());

    expect(state).toEqual({loading: false});
});

test('when failed action received then request is not loading', () => {
    let state = loadingReducer(undefined, initAction());
    state = loadingReducer(state, testLoadingActions.request());
    state = loadingReducer(state, testLoadingActions.failed());

    expect(state).toEqual({loading: false});
});

test('when getting loading state then returns loading state for async action', () => {
    let state = loadingReducer(undefined, initAction());
    state = loadingReducer(state, testLoadingActions.request());
    
    const appState = { loading: state };
    expect(selectIsLoading(appState, testLoadingActions.request)).toEqual(true);
})