import {Mask} from "msw/lib/types/setupWorker/glossary";
import {MockedRequest, rest} from "msw";
import {DEFAULT_DELAY, DEFAULT_GET_STATUS, DEFAULT_POST_STATUS, RequestOptions} from "./request-options";
import {TestingServer, server} from "./server";

function captureRequest(req: MockedRequest) {
  const request = {url: req.url.toString(), body: req.body, method: req.method};
  TestingServer.requests.push(request);
}

export function setupHttpGet(url: Mask, result: any, {status = DEFAULT_GET_STATUS, delay = DEFAULT_DELAY}: RequestOptions = {}) {
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

export function setupHttpPost(url: Mask, data: any, {status = DEFAULT_POST_STATUS, delay = DEFAULT_DELAY}: RequestOptions = {}) {
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
