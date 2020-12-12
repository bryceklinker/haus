import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {map} from "rxjs/operators";
import {Observable} from "rxjs";

import {ListResult} from "../models";
import {DeviceModel} from "../../devices/models";

@Injectable()
export class HausApiClient {
  constructor(private http: HttpClient) {
  }

  getDevices(): Observable<ListResult<DeviceModel>> {
    return this.http.get<ListResult<DeviceModel>>('/api/devices');
  }

  turnDeviceOff(deviceId: number): Observable<void> {
    return this.postEmpty(`/api/devices/${deviceId}/turn-off`);
  }

  turnDeviceOn(deviceId: number): Observable<void> {
    return this.postEmpty(`/api/devices/${deviceId}/turn-on`);
  }

  startDiscovery(): Observable<void> {
    return this.postEmpty('/api/devices/start-discovery');
  }

  stopDiscovery(): Observable<void> {
    return this.postEmpty('/api/devices/stop-discovery');
  }

  syncDiscovery(): Observable<void> {
    return this.postEmpty('/api/devices/sync-discovery');
  }

  postEmpty(path: string): Observable<void> {
    return this.http.post(path, null).pipe(map(() => {}));
  }
}
