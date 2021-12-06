import {SignalrHubConnection} from '../../app/shared/signalr';
import {Subject} from 'rxjs';
import {HubConnectionState} from '@microsoft/signalr';
import {tap} from 'rxjs/operators';

export class TestingSignalrHubConnection extends SignalrHubConnection {
  private _startSubject = new Subject<void>();
  private _stopSubject = new Subject<void>();
  private _handlers: { [methodName: string]: Subject<any> } = {};
  private _state: HubConnectionState = HubConnectionState.Disconnected;

  get state(): HubConnectionState {
    return this._state;
  }

  constructor(public hubName: string) {
    super(<any>jest.fn());

    this._startSubject.pipe(tap(() => this._state = HubConnectionState.Connected));
    this._stopSubject.pipe(tap(() => this._state = HubConnectionState.Disconnected));
    jest.spyOn(this as TestingSignalrHubConnection, 'start');
    jest.spyOn(this as TestingSignalrHubConnection, 'stop');
  }

  start() {
    return this._startSubject.asObservable();
  }

  stop() {
    return this._stopSubject.asObservable();
  }

  on(methodName: string, handler: (...args: any[]) => void) {
    this.getSubjectForMethodName(methodName).subscribe(handler);
  }

  triggerStart() {
    this._startSubject.next();
  }

  triggerStop() {
    this._stopSubject.next();
  }

  triggerMessage(methodName: string, ...args: any[]) {
    // @ts-ignore
    this.getSubjectForMethodName(methodName).next(...args);
  }

  triggerMqttMessage(...args: any[]) {
    this.triggerMessage('OnMqttMessage', ...args);
  }

  private getSubjectForMethodName(methodName: string): Subject<any> {
    return this._handlers.hasOwnProperty(methodName)
      ? this._handlers[methodName]
      : (this._handlers[methodName] = new Subject<any>());
  }
}
