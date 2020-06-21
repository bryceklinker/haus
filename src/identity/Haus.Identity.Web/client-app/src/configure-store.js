import React from 'react';
import {applyMiddleware, combineReducers, compose, createStore} from 'redux';
import {createSagaMiddleware} from 'redux-saga';
import {createDevTools} from 'redux-devtools';
import DockMonitor from 'redux-devtools-dock-monitor';
import LogMonitor from 'redux-devtools-log-monitor';
import {usersReducer} from './users/state/users.reducer';
import {loadingReducer} from './shared/loading/loading.reducer';

export function createRootReducer() {
    return combineReducers({
        users: usersReducer,
        loading: loadingReducer
    });
}

export function* rootSaga() {

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
    const sagaMiddleware = createSagaMiddleware();
    const enhancer = compose(
        applyMiddleware(sagaMiddleware),
        DevTools.instrument()
    );
    const store = createStore(reducer, {}, enhancer);
    sagaMiddleware.run(rootSaga);
    return store;
}