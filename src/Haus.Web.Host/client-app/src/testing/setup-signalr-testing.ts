import {enableTesting} from 'ngrx-signalr-core';
import {IHttpConnectionOptions} from "@microsoft/signalr";
import {TestingHub} from "./fakes";

const hubs: Array<TestingHub> = [];

export function setupSignalrTestingHub() {
  enableTesting((hubName: string, url: string, options?: IHttpConnectionOptions) => {
    const hub = new TestingHub(hubName, url, options);
    hubs.push(hub);
    return hub;
  });
}

