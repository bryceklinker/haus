import {Injectable} from "@angular/core";
import {SignalrService} from "./signalr.service";
import {SignalrHubConnectionFactory} from "./signalr-hub-connection-factory";

@Injectable()
export class SignalrServiceFactory {
  constructor(private readonly connectionFactory: SignalrHubConnectionFactory) {
  }

  create(hubName: string): SignalrService {
    const connection = this.connectionFactory.create(hubName);
    return new SignalrService(connection);
  }
}
