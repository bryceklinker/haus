import {setupServer, rest} from 'msw/node';
import {TEST_SETTINGS} from './test-settings';

const server = setupServer(
    rest.get('/settings.json', (req, res, ctx) => {
        return res(ctx.json(TEST_SETTINGS));
    })
);

function start() {
    server.listen();
}

function stop() {
    server.close();
}

function reset() {
    server.resetHandlers();
}

function setupGet(url, result) {
    server.use(
        rest.get(url, (req, res, ctx) => {
            res(ctx.json(result));
        })
    );
}

export const TestingServer = {start, stop, reset, setupGet};
