import {initAction} from '../../testing/testing.actions';
import {createModalActions} from '../state/actions';
import {modalReducer, selectIsModalOpen} from './modals.reducer';

const modalActions = createModalActions('modals');

test('when initialized then state is empty', () => {
    const state = modalReducer(undefined, initAction());

    expect(state).toEqual({});
});

test('when modal opened then modal for action is in opened state', () => {
    let state = modalReducer(undefined, initAction());
    state = modalReducer(state, modalActions.open());

    expect(state['modals']).toEqual(true);
});

test('when modal cancelled then modal for action is closed', () => {
    let state = modalReducer(undefined, initAction());
    state = modalReducer(state, modalActions.open());
    state = modalReducer(state, modalActions.cancel());

    expect(state['modals']).toEqual(false);
});

test('when modal succeeds then modal for action is closed', () => {
    let state = modalReducer(undefined, initAction());
    state = modalReducer(state, modalActions.open());
    state = modalReducer(state, modalActions.success());

    expect(state['modals']).toEqual(false);
});

test('when selecting modal state then returns modal state for modal action', () => {
    let state = modalReducer(undefined, initAction());
    state = modalReducer(state, modalActions.open());
    
    const appState = {modals: state};
    expect(selectIsModalOpen(appState, modalActions.request)).toEqual(true);
})