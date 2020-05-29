import server from './server';
import fetch from 'node-fetch';

test('when server started then listening on specified port', async () => {
    await server.start(3000);

    const response = await fetch('http://localhost:3000/.well-known/openid-configuration');

    expect(response.status).toBe(200);
})

afterEach(async () => {
    await server.stop();
})