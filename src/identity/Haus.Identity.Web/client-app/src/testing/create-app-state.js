import {createRootReducer} from '../configure-store';
import {initAction} from './testing.actions';

export function createAppState(...actions) {
    const reducer = createRootReducer();
    const initialState = reducer(undefined, initAction());
    return actions.reduce((state, action) => reducer(state, action), initialState);
}