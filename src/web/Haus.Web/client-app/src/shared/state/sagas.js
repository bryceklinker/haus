import {getContext} from '@redux-saga/core/effects';

export function createSagaMiddlewareOptions(requestor = fetch) {
    return {
        context: {requestor}
    };
}

export function getRequestor() {
    return getContext('requestor');
}