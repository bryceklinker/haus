import {SignalRTestingHub} from "ngrx-signalr-core";
import {Observable, Subject} from "rxjs";
import {IHttpConnectionOptions} from "@microsoft/signalr";

export class TestingHub extends SignalRTestingHub {
  private _eventSubjects: { [eventName: string]: Subject<any> } = {};

  constructor(hubName: string, url: string, options?: IHttpConnectionOptions) {
    super(hubName, url, options);
  }

  publish<T = any>(eventName: string, value: T) {
    this.getSubjectForEvent(eventName).next(value);
  }
  
  off(eventName: string): void {
    this.getSubjectForEvent(eventName).complete();
  }

  on<T>(eventName: string): Observable<T>;
  on<T>(eventName: string): Observable<T>;
  on(eventName: string): Observable<any> {
    return this.getSubjectForEvent(eventName).asObservable();
  }

  send<T>(methodName: string, ...args: any[]): Observable<T>;
  send<T>(methodName: string, ...args: any[]): Observable<T>;
  send(methodName: string, ...args: any[]): Observable<any> {
    throw new Error('Not Implemented');
  }

  sendStream<T>(methodName: string, observable: Observable<T>): void;
  sendStream<T>(methodName: string, observable: Observable<T>): void;
  sendStream(methodName: string, observable: Observable<any>): void {
    throw new Error('Not Implemented');
  }

  stream<T>(methodName: string, ...args: any[]): Observable<T>;
  stream<T>(methodName: string, ...args: any[]): Observable<T>;
  stream(methodName: string, ...args: any[]): Observable<any> {
    throw new Error('Not Implemented');
  }

  private getSubjectForEvent(eventName: string): Subject<any> {
    if (this._eventSubjects.hasOwnProperty(eventName)) {
      return this._eventSubjects[eventName];
    }

    this._eventSubjects[eventName] = new Subject<any>();
    return this._eventSubjects[eventName];
  }
}
