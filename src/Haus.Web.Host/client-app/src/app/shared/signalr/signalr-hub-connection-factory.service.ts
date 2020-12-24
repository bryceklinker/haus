import {SignalrHubConnection} from "./signalr-hub.connection";
import {HubConnectionBuilder, LogLevel} from "@microsoft/signalr";
import {Injectable} from "@angular/core";
import {AuthService} from "@auth0/auth0-angular";
import {fromObservableToPromise} from "../observable-extensions";

@Injectable()
export class SignalrHubConnectionFactory {
  constructor(private auth: AuthService) {

  }

  create(hubName: string): SignalrHubConnection {
    const connection = new HubConnectionBuilder()
      .withUrl(`/hubs/${hubName}`, {
        accessTokenFactory: () => fromObservableToPromise(this.auth.getAccessTokenSilently())
      })
      .configureLogging(LogLevel.Information)
      .build();
    return new SignalrHubConnection(connection);
  }

  createFromUrl(url: string): SignalrHubConnection {
    const connection = new HubConnectionBuilder()
      .withUrl(url)
      .configureLogging(LogLevel.Information)
      .build();
    return new SignalrHubConnection(connection);
  }
}
