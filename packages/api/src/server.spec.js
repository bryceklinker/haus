import superagent from 'superagent';
import {Server} from './server';

const PORT = 8000;
const BASE_URL = `http://localhost:${PORT}`;

describe('server', () => {
    beforeAll(async () => {
        await Server.start(PORT);
    });

    test('when getting health then returns status of 200', async () => {
        const response = await superagent.get(`${BASE_URL}/.health`);

        expect(response.status).toEqual(200);
    });

    afterAll(() => {
        Server.stop();
    });
});
