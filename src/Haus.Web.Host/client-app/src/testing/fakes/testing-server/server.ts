import {setupServer} from "msw/node";

type Request = { url: string, method: string, body: any }
let requests: Array<Request> = [];
export const server = setupServer();

function start() {
  server.listen({onUnhandledRequest: 'warn'});
}

function stop() {
  server.close();
}

function reset() {
  server.resetHandlers();
  requests = [];
}

export const TestingServer = {
  start,
  stop,
  reset,
  get requests(): Request[] {
    return requests;
  },
  get lastRequest(): Request {
    return TestingServer.requests[TestingServer.requests.length - 1];
  }
}
