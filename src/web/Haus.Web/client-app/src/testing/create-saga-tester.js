import SagaTester from 'redux-saga-tester';
import {createSagaMiddlewareOptions} from '../shared/state/sagas';

export function createSagaTester() {
    const requestor = createRequestor();
    const sagaTester = new SagaTester({
        options: createSagaMiddlewareOptions(requestor)
    });

    return {sagaTester, requestor};
}

function createRequestor() {
    const configuredRequests = [];
    const handledRequests = [];
    
    const getRequestValues = (urlOrInit, init) => {
        const isUrl = typeof urlOrInit === 'string';
        const url = isUrl ? urlOrInit : urlOrInit.url;
        
        let method = 'GET';
        if (isUrl && init && init.method) {
            method = init.method;
        } else if (urlOrInit.method) {
            method = urlOrInit.method;
        }

        let body = null;
        if (isUrl && init && init.body) {
            body = init.body;
        } else if (urlOrInit.body) {
            body = urlOrInit.body;
        }
        
        return {url, method, body};
    };

    function requestor(urlOrInit, init = undefined) {
        const {url, method, body} = getRequestValues(urlOrInit, init);
        handledRequests.push({url, method, body});
        const configuredRequest = configuredRequests.find(r => r.url === url && r.method === method);
        if (configuredRequest) {
            return Promise.resolve(new Response(configuredRequest.body, {status: configuredRequest.status}));
        }

        throw new Error(`No request configured for [${method}] ${url}.`);
    };

    requestor.getRequests = () => {
        return handledRequests;
    }

    requestor.setupResponse = (method, url, body, status = 200) => {
        configuredRequests.push({method, url, body, status});
    };

    requestor.setupGet = (url, body, status = 200) => {
        requestor.setupResponse('GET', url, body, status);
    };
    
    requestor.setupPost = (url, responseBody, status = 200) => {
        requestor.setupResponse('POST', url, responseBody, status);
    }

    requestor.setupPut = (url, responseBody, status = 200) => {
        requestor.setupResponse('PUT', url, responseBody, status);
    }

    requestor.setupDelete = (url, responseBody, status = 200) => {
        requestor.setupResponse('DELETE', url, responseBody, status);
    }
    return requestor;
}