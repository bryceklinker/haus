import {MockedRequest, rest,} from "msw";
import {DEFAULT_DELAY, DEFAULT_GET_STATUS, DEFAULT_POST_STATUS, RequestOptions} from "./request-options";
import {TestingServer, server} from "./server";

type Path = string | RegExp;

function captureRequest(req: MockedRequest) {
  const request = {url: req.url.toString(), body: req.body, method: req.method};
  TestingServer.requests.push(request);
}

export function setupHttpGet(url: Path, result: any, {status = DEFAULT_GET_STATUS, delay = DEFAULT_DELAY}: RequestOptions = {}) {
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

export function setupHttpPost(url: Path, data: any, {status = DEFAULT_POST_STATUS, delay = DEFAULT_DELAY}: RequestOptions = {}) {
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

export function setupHttpPut(url: Path, data: any, {status = DEFAULT_POST_STATUS, delay = DEFAULT_DELAY}: RequestOptions = {}) {
  server.use(
    rest.put(url, (req, res, ctx) => {
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
