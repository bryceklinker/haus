import React from 'react';
import {applyMiddleware, combineReducers, compose, createStore} from 'redux';
import createSagaMiddleware from 'redux-saga';
import {createDevTools} from 'redux-devtools';
import DockMonitor from 'redux-devtools-dock-monitor';
import LogMonitor from 'redux-devtools-log-monitor';
import {usersReducer} from './users/state/users.reducer';
import {loadingReducer} from './shared/loading/loading.reducer';
import {modalReducer} from './shared/modals/modals.reducer';
import {createSagaMiddlewareOptions} from './shared/state/sagas';
import {all} from '@redux-saga/core/effects';
import {usersSaga} from './users/state/users.sagas';

export function createRootReducer() {
    return combineReducers({
        users: usersReducer,
        loading: loadingReducer,
        modals: modalReducer
    });
}

export function* rootSaga() {
    yield all([
        usersSaga()
    ])
}

const DevTools = createDevTools(
    <DockMonitor
        toggleVisibilityKey={'ctrl-h'}
        changePositionKey={'ctrl-q'}
        defaultIsVisible={true}>
        <LogMonitor theme={'tomorrow'}/>
    </DockMonitor>
);

export function configureStore() {
    const reducer = createRootReducer();
    const sagaMiddleware = createSagaMiddleware(createSagaMiddlewareOptions());
    const enhancer = compose(
        applyMiddleware(sagaMiddleware),
        DevTools.instrument()
    );
    const store = createStore(reducer, {}, enhancer);
    sagaMiddleware.run(rootSaga);
    return store;
}