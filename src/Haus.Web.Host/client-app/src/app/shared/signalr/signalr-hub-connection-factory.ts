import {HubConnectionBuilder, LogLevel} from "@microsoft/signalr";
import {Injectable} from "@angular/core";
import {SignalrHubConnection} from "./signalr-hub.connection";
import {fromObservableToPromise} from "../observable-extensions";
import {HausAuthService} from '../auth/services';

@Injectable()
export class SignalrHubConnectionFactory {
  constructor(private authService: HausAuthService) {

  }

  create(hubName: string): SignalrHubConnection {
    const connection = new HubConnectionBuilder()
      .withUrl(`/hubs/${hubName}`, {
        accessTokenFactory: () => fromObservableToPromise(this.authService.token$),
        withCredentials: false,
      })
      .configureLogging(LogLevel.Information)
      .build();
    return new SignalrHubConnection(connection);
  }
}
