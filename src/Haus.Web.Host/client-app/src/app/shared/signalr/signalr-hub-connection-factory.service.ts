import {SignalrHubConnection} from "./signalr-hub.connection";
import {HubConnectionBuilder, LogLevel} from "@microsoft/signalr";
import {Injectable} from "@angular/core";

@Injectable()
export class SignalrHubConnectionFactory {
  create(hubName: string): SignalrHubConnection {
    const connection = new HubConnectionBuilder()
      .withUrl(`/hubs/${hubName}`)
      .configureLogging(LogLevel.Information)
      .build();
    return new SignalrHubConnection(connection);
  }
}
