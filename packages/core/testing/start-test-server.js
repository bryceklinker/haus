import superagent from 'superagent';
import {createDbFactory} from '../src/common/database';
import testSettings from './test-settings';
import {bootstrapApp} from '../src';
import {configureRestApi} from '../src/app/configure-rest-api';

function resolveUrl(baseUrl, path) {
    baseUrl = baseUrl.endsWith('/')
        ? baseUrl.substring(0, baseUrl.length - 1)
        : baseUrl;

    path = path.startsWith('/')
        ? path.substring(1)
        : path;

    return `${baseUrl}/${path}`
}

function createServerClient(baseUrl) {
    return {
        get: (path) => superagent.get(resolveUrl(baseUrl, path)),
        post: (path) => superagent.post(resolveUrl(baseUrl, path)),
        put: (path) => superagent.put(resolveUrl(baseUrl, path)),
        delete: (path) => superagent.delete(resolveUrl(baseUrl, path))
    }
}

export async function startTestRestApi(settings = testSettings) {
    const app = configureRestApi(settings);
    return {

    }
}

export async function startTestServer() {
    const app = bootstrapApp(testSettings);
    await app.start(0);
    return {
        ...app,
        httpClient: createServerClient(app.baseUrl),
        createDb: createDbFactory(testSettings)
    };
}