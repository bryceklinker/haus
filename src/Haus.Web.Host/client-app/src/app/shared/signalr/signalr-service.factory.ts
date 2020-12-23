import {Injectable} from "@angular/core";
import {SignalrService} from "./signalr.service";
import {SignalrHubConnectionFactory} from "./signalr-hub-connection-factory.service";

@Injectable()
export class SignalrServiceFactory {
  constructor(private readonly connectionFactory: SignalrHubConnectionFactory) {
  }

  create(hubName: string): SignalrService {
    return new SignalrService(hubName, this.connectionFactory);
  }
}
