import {createAppState} from './create-app-state';
import configureStore from 'redux-mock-store';

export function createTestingStore(...actions) {
    const state = createAppState(...actions);
    return configureStore([])(state);
}