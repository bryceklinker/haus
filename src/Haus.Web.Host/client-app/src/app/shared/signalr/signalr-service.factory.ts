import {Injectable} from "@angular/core";
import {SignalrService} from "./signalr.service";
import {SignalrHubConnectionFactory} from "./signalr-hub-connection-factory.service";

@Injectable()
export class SignalrServiceFactory {
  constructor(private readonly connectionFactory: SignalrHubConnectionFactory) {
  }

  create(hubName: string): SignalrService {
    const connection = this.connectionFactory.create(hubName);
    return new SignalrService(connection);
  }

  createFromUrl(url: string): SignalrService {
    const connection = this.connectionFactory.createFromUrl(url);
    return new SignalrService(connection);
  }
}
