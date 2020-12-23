import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {catchError, map, tap} from "rxjs/operators";
import {BehaviorSubject, Observable} from "rxjs";
import {v4 as uuid} from 'uuid';

import {ListResult} from "../models";
import {RoomModel} from "../rooms";
import {DeviceModel} from "../devices";
import {DiagnosticsMessageModel} from "../diagnostics";

enum HttpMethod {
  GET = 'GET',
  POST = 'POST'
}

@Injectable()
export class HausApiClient {
  private readonly _inflightRequests = new BehaviorSubject<Array<string>>([]);

  get inflightRequests(): Array<string> {
    return this._inflightRequests.getValue();
  }
  get isLoading$(): Observable<boolean> {
    return this._inflightRequests.pipe(
      map(requests => requests.length > 0)
    )
  }

  constructor(private http: HttpClient) {
  }

  getDevices(): Observable<ListResult<DeviceModel>> {
    return this.execute<ListResult<DeviceModel>>(HttpMethod.GET, '/api/devices');
  }

  turnDeviceOff(deviceId: number): Observable<void> {
    return this.execute(HttpMethod.POST, `/api/devices/${deviceId}/turn-off`);
  }

  turnDeviceOn(deviceId: number): Observable<void> {
    return this.execute(HttpMethod.POST, `/api/devices/${deviceId}/turn-on`);
  }

  startDiscovery(): Observable<void> {
    return this.execute(HttpMethod.POST, '/api/devices/start-discovery');
  }

  stopDiscovery(): Observable<void> {
    return this.execute(HttpMethod.POST, '/api/devices/stop-discovery');
  }

  syncDiscovery(): Observable<void> {
    return this.execute(HttpMethod.POST, '/api/devices/sync-discovery');
  }

  getRooms(): Observable<ListResult<RoomModel>> {
    return this.execute<ListResult<RoomModel>>(HttpMethod.GET, '/api/rooms');
  }

  addRoom(room: RoomModel): Observable<RoomModel> {
    return this.execute<RoomModel>(HttpMethod.POST, '/api/rooms', room);
  }

  replayMessage(message: DiagnosticsMessageModel): Observable<void> {
    return this.execute(HttpMethod.POST, '/api/diagnostics/replay', message).pipe(map(() => {}));
  }

  private execute<T>(method: HttpMethod, url: string, data: any = null): Observable<T> {
    const requestId = uuid();
    this.addInflightRequest(requestId);
    return this.executeHttpMethod<T>(method, url, data).pipe(
      tap(() => this.removeInflightRequest(requestId))
    );
  }

  private executeHttpMethod<T>(method: HttpMethod, url: string, data: any): Observable<T> {
    switch (method.toUpperCase()) {
      case 'GET':
        return this.http.get<T>(url);
      case 'POST':
        return this.http.post<T>(url, data);
      default:
        throw new Error(`Unrecognized http method: ${method}`);
    }
  }

  private addInflightRequest(requestId: string) {
    this._inflightRequests.next([...this.inflightRequests, requestId]);
  }

  private removeInflightRequest(requestId: string) {
    this._inflightRequests.next([...this.inflightRequests.filter(r => r !== requestId)]);
  }
}
