import {SignalrHubConnection} from "../../app/shared/signalr/signalr-hub.connection";
import {SignalrHubConnectionFactory} from "../../app/shared/signalr/signalr-hub-connection-factory.service";
import {TestingSignalrHubConnectionService} from "./testing-signalr-hub-connection.service";

export class TestingSignalrConnectionServiceFactory extends SignalrHubConnectionFactory {
  private _connections: {[hubName: string]: TestingSignalrHubConnectionService} = {};

  create(hubName: string): SignalrHubConnection {
    return this.getTestingHub(hubName);
  }

  createFromUrl(url: string): SignalrHubConnection {
    return this.getTestingHub(url);
  }

  getTestingHub(hubName: string): TestingSignalrHubConnectionService {
    return this._connections.hasOwnProperty(hubName)
      ? this._connections[hubName]
      : (this._connections[hubName] = new TestingSignalrHubConnectionService(hubName));
  }
}
