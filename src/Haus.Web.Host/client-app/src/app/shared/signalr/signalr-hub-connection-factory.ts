import {HubConnectionBuilder, LogLevel} from "@microsoft/signalr";
import {Injectable, Injector} from "@angular/core";
import {AuthService} from "@auth0/auth0-angular";
import {SignalrHubConnection} from "./signalr-hub.connection";
import {fromObservableToPromise} from "../observable-extensions";

@Injectable()
export class SignalrHubConnectionFactory {
  constructor(private injector: Injector) {

  }

  create(hubName: string): SignalrHubConnection {
    const authService = this.injector.get(AuthService);
    const connection = new HubConnectionBuilder()
      .withUrl(`/hubs/${hubName}`, {
        accessTokenFactory: () => fromObservableToPromise(authService.getAccessTokenSilently())
      })
      .configureLogging(LogLevel.Information)
      .build();
    return new SignalrHubConnection(connection);
  }
}
