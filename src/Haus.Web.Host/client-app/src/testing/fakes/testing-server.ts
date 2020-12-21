import {setupServer} from 'msw/node';
import {rest} from 'msw';
import {ModelFactory} from "../model-factory";

function createDefaultServer() {
  return setupServer(
    rest.get('/api/rooms', (req, res, ctx) => {
      return res(
        ctx.json(ModelFactory.createListResult()),
        ctx.status(200)
      )
    }),
    rest.post('/api/rooms', (req, res, ctx) => {
      return res(
        ctx.json(ModelFactory.createRoomModel()),
        ctx.status(204)
      )
    }),
    rest.get('/api/devices', (req, res, ctx) => {
      return res(
        ctx.json(ModelFactory.createListResult()),
        ctx.status(200)
      )
    }),
    rest.post('/api/devices/sync-discovery', (req, res, ctx) => {
      return res(
        ctx.status(204)
      )
    }),
    rest.post('/api/devices/stop-discovery', (req, res, ctx) => {
      return res(
        ctx.status(204)
      )
    }),
    rest.post('/api/devices/start-discovery', (req, res, ctx) => {
      return res(
        ctx.status(204)
      )
    }),
    rest.post('/api/diagnostics/replay', (req, res, ctx) => {
      return res(
        ctx.status(204)
      )
    })
  )
}
const server = createDefaultServer();

function start() {
  server.listen({onUnhandledRequest: 'warn'});
}
function stop() {

  server.close();
}
function setupGet(url: string, result: any, status: number = 200) {

  server.use(
    rest.get(url, (req, res, ctx) => {
      return res(
        ctx.status(status),
        ctx.json(result)
      );
    })
  )
}
function setupPost(url: string, data: any, status: number = 201) {

  server.use(
    rest.post(url, (req, res, ctx) => {
      if (data) {
        return res(
          ctx.json(data),
          ctx.status(status)
        );
      } else {
        return res(
          ctx.status(status)
        );
      }
    })
  )
}
function reset() {
  server.resetHandlers();
}


export const TestingServer = {
  start,
  stop,
  reset,
  setupPost,
  setupGet
}
