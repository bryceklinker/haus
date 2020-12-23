import {HubConnection, HubConnectionState} from "@microsoft/signalr";
import {Observable} from "rxjs";
import {fromPromise} from "rxjs/internal-compatibility";

export class SignalrHubConnection {
  get state(): HubConnectionState {
    return this.connection.state;
  }

  constructor(private connection: HubConnection) {
  }

  start(): Observable<void> {
    return fromPromise(this.connection.start());
  }

  stop(): Observable<void> {
    return fromPromise(this.connection.stop());
  }

  on(methodName: string, handler: (...args: any[]) => void): void {
    this.connection.on(methodName, handler);
  }
}
