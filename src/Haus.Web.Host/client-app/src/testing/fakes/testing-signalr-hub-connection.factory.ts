import {SignalrHubConnection} from "../../app/shared/signalr";
import {SignalrHubConnectionFactory} from "../../app/shared/signalr";
import {TestingSignalrHubConnection} from "./testing-signalr-hub.connection";

export class TestingSignalrHubConnectionFactory extends SignalrHubConnectionFactory {
  private _connections: {[hubName: string]: TestingSignalrHubConnection} = {};

  create(hubName: string): SignalrHubConnection {
    return this.getTestingHub(hubName);
  }

  createFromUrl(url: string): SignalrHubConnection {
    return this.getTestingHub(url);
  }

  getTestingHub(hubName: string): TestingSignalrHubConnection {
    return this._connections.hasOwnProperty(hubName)
      ? this._connections[hubName]
      : (this._connections[hubName] = new TestingSignalrHubConnection(hubName));
  }
}
