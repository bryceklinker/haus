import {HubConnection, HubConnectionState} from "@microsoft/signalr";
import {Observable, from} from "rxjs";

export class SignalrHubConnection {
  get state(): HubConnectionState {
    return this.connection.state;
  }

  constructor(private connection: HubConnection) {
  }

  start(): Observable<void> {
    return from(this.connection.start());
  }

  stop(): Observable<void> {
    return from(this.connection.stop());
  }

  on(methodName: string, handler: (...args: any[]) => void): void {
    this.connection.on(methodName, handler);
  }
}
