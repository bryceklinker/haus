import {setupServer} from 'msw/node';
import {MockedRequest, rest} from 'msw';
import {Mask} from "msw/lib/types/setupWorker/glossary";
import {ModelFactory} from "../model-factory";

type Request = { url: string, method: string, body: any }
let requests: Array<Request> = [];
const server = setupServer();

function start() {
  server.listen({onUnhandledRequest: 'warn'});
}

function stop() {
  server.close();
}

function captureRequest(req: MockedRequest) {
  const request = {url: req.url.toString(), body: req.body, method: req.method};
  requests.push(request);
}

function setupGet(url: Mask, result: any, {status = 200, delay = 0} = {}) {
  server.use(
    rest.get(url, (req, res, ctx) => {
      captureRequest(req);
      return res(
        ctx.status(status),
        ctx.delay(delay),
        ctx.json(result)
      );
    })
  )
}

function setupPost(url: Mask, data: any, {status = 201, delay = 0} = {}) {
  server.use(
    rest.post(url, (req, res, ctx) => {
      captureRequest(req);
      const transforms = [
        ctx.status(status),
        ctx.delay(delay)
      ]

      if (data) {
        transforms.push(ctx.json(data));
      }
      return res(...transforms);
    })
  )
}

function reset() {
  server.resetHandlers();
  requests = [];
}

function setupRoomsEndpoints() {
  setupGet('/api/rooms', ModelFactory.createListResult());
  setupPost('/api/rooms', ModelFactory.createRoomModel());
}

function setupDevicesEndpoints() {
  setupGet('/api/devices', ModelFactory.createListResult());
  setupPost('/api/devices/start-discovery', null, {status: 204});
  setupPost('/api/devices/stop-discovery', null, {status: 204});
  setupPost('/api/devices/sync-discovery', null, {status: 204});
  setupPost(/\/api\/devices\/*\/turn-off/, null, {status: 204});
  setupPost(/\/api\/devices\/*\/turn-on/, null, {status: 204});
}

function setupDiagnosticEndpoints() {
  setupPost('/api/diagnostics/replay', null, {status: 204});
}

function setupDefaultApiEndpoints() {
  setupDevicesEndpoints();
  setupDiagnosticEndpoints();
}

export const TestingServer = {
  start,
  stop,
  reset,
  setupPost,
  setupGet,
  setupDevicesEndpoints,
  setupDiagnosticEndpoints,
  setupDefaultApiEndpoints,
  setupRoomsEndpoints,
  get requests(): Request[] {
    return requests;
  },
  get lastRequest(): Request {
    return TestingServer.requests[TestingServer.requests.length - 1];
  }
};
