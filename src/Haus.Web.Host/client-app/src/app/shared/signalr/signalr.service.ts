import {BehaviorSubject, Observable} from "rxjs";
import {HubStatus} from "../models";
import {SignalrHubConnection} from "./signalr-hub.connection";
import {SignalrHubConnectionFactory} from "./signalr-hub-connection-factory.service";

export class SignalrService {
  private readonly _status = new BehaviorSubject<HubStatus>(HubStatus.Disconnected);
  private _connection: SignalrHubConnection | null = null;

  get status$(): Observable<HubStatus> {
    return this._status.asObservable();
  }

  private get connection(): SignalrHubConnection {
    return this._connection || (this._connection = this.signalrConnectionFactory.create(this.hubName));
  }

  constructor(public readonly hubName: string,
              private readonly signalrConnectionFactory: SignalrHubConnectionFactory) {
  }

  start(): Observable<void> {
    this.updateStatus(HubStatus.Connecting);
    return this.executeAndUpdateStatus(() => this.connection.start(), HubStatus.Connected);
  }

  stop(): Observable<void> {
    return this.executeAndUpdateStatus(() => this.connection.stop(), HubStatus.Disconnected);
  }

  private executeAndUpdateStatus(func: () => Observable<void>, status: HubStatus): Observable<void> {
    const obs$ = func();
    const subscription = obs$.subscribe(() => {
      this.updateStatus(status);
      subscription.unsubscribe();
    })
    return obs$;
  }

  private updateStatus(status: HubStatus) {
    this._status.next(status);
  }

  on<T>(methodName: string, handler: (arg: T) => void) {
    this.connection.on(methodName, handler);
  }
}
