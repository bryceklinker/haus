import {Injectable, OnDestroy} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {map, tap} from "rxjs/operators";
import {BehaviorSubject, Observable} from "rxjs";
import {v4 as uuid} from 'uuid';

import {
  DeviceModel,
  DeviceType,
  DiscoveryModel,
  LightingModel,
  ListResult,
  MqttDiagnosticsMessageModel,
  RoomModel,
  SimulatedDeviceModel
} from "../models";
import {HttpMethod} from "./http-method";
import {DestroyableSubject} from "../destroyable-subject";

@Injectable({
  providedIn: 'root'
})
export class HausApiClient implements OnDestroy {
  private readonly destroyable = new DestroyableSubject();
  private readonly _inflightRequests = new BehaviorSubject<Array<string>>([]);

  get inflightRequests(): Array<string> {
    return this._inflightRequests.getValue();
  }
  get isLoading$(): Observable<boolean> {
    return this.destroyable.register(this._inflightRequests.pipe(
      map(requests => requests.length > 0),
    ));
  }

  constructor(private http: HttpClient) {
  }

  getDevices(): Observable<ListResult<DeviceModel>> {
    return this.execute<ListResult<DeviceModel>>(HttpMethod.GET, '/api/devices');
  }

  getDeviceTypes(): Observable<ListResult<DeviceType>> {
    return this.execute<ListResult<DeviceType>>(HttpMethod.GET, '/api/device-types');
  }

  turnDeviceOff(deviceId: number): Observable<void> {
    return this.execute(HttpMethod.POST, `/api/devices/${deviceId}/turn-off`);
  }

  turnDeviceOn(deviceId: number): Observable<void> {
    return this.execute(HttpMethod.POST, `/api/devices/${deviceId}/turn-on`);
  }

  startDiscovery(): Observable<void> {
    return this.execute(HttpMethod.POST, '/api/discovery/start');
  }

  stopDiscovery(): Observable<void> {
    return this.execute(HttpMethod.POST, '/api/discovery/stop');
  }

  syncDiscovery(): Observable<void> {
    return this.execute(HttpMethod.POST, '/api/discovery/sync');
  }

  getDiscovery(): Observable<DiscoveryModel> {
    return this.execute<DiscoveryModel>(HttpMethod.GET, '/api/discovery/state');
  }

  getRooms(): Observable<ListResult<RoomModel>> {
    return this.execute<ListResult<RoomModel>>(HttpMethod.GET, '/api/rooms');
  }

  addRoom(room: Partial<RoomModel>): Observable<RoomModel> {
    return this.execute<RoomModel>(HttpMethod.POST, '/api/rooms', room);
  }

  assignDevicesToRoom(roomId: number, deviceIds: Array<number>): Observable<void> {
    return this.execute(HttpMethod.POST, `/api/rooms/${roomId}/add-devices`, deviceIds);
  }

  changeRoomLighting(roomId: number, lighting: LightingModel): Observable<void> {
    return this.execute(HttpMethod.PUT, `/api/rooms/${roomId}/lighting`, lighting);
  }

  getDevicesInRoom(roomId: string): Observable<ListResult<DeviceModel>> {
    return this.execute<ListResult<DeviceModel>>(HttpMethod.GET, `/api/rooms/${roomId}/devices`)
  }

  replayMessage(message: MqttDiagnosticsMessageModel): Observable<void> {
    return this.execute(HttpMethod.POST, '/api/diagnostics/replay', message);
  }

  addSimulatedDevice(model: Partial<SimulatedDeviceModel>): Observable<void> {
    return this.execute(HttpMethod.POST, '/api/device-simulator/devices', model);
  }

  ngOnDestroy(): void {
    this.destroyable.destroy();
  }

  private execute<T>(method: HttpMethod, url: string, data: any = null): Observable<T> {
    const requestId = uuid();
    this.addInflightRequest(requestId);
    return this.destroyable.register(this.executeHttpMethod<T>(method, url, data).pipe(
      tap(() => this.removeInflightRequest(requestId)),
    ));
  }

  private executeHttpMethod<T>(method: HttpMethod, url: string, data: any): Observable<T> {
    switch (method.toUpperCase()) {
      case 'GET':
        return this.http.get<T>(url);
      case 'POST':
        return this.http.post<T>(url, data);
      case 'PUT':
        return this.http.put<T>(url, data);
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
