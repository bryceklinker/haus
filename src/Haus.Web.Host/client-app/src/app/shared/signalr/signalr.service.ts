import {BehaviorSubject, Observable} from "rxjs";
import {HubStatus} from "../models";
import {SignalrHubConnection} from "./signalr-hub.connection";
import {SignalrHubConnectionFactory} from "./signalr-hub-connection-factory.service";
import {takeUntil, tap} from "rxjs/operators";
import {DestroyableSubject} from "../destroyable-subject";

export class SignalrService {
  private readonly _unsubscribe$ = new DestroyableSubject();
  private readonly _status = new BehaviorSubject<HubStatus>(HubStatus.Disconnected);
  private _connection: SignalrHubConnection | null = null;

  get status$(): Observable<HubStatus> {
    return this._status.asObservable().pipe(
      takeUntil(this._unsubscribe$)
    );
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

  destroy(): void {
    this._unsubscribe$.destroy();
  }

  private executeAndUpdateStatus(func: () => Observable<void>, status: HubStatus): Observable<void> {
    return func().pipe(
      tap(() => this.updateStatus(status)),
      takeUntil(this._unsubscribe$)
    )
  }

  private updateStatus(status: HubStatus) {
    this._status.next(status);
  }

  on<T>(methodName: string, handler: (arg: T) => void) {
    this.connection.on(methodName, handler);
  }
}
