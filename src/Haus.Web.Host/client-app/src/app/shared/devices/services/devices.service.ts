import {Injectable} from "@angular/core";
import {Observable} from "rxjs";
import {DeviceModel} from "../models";
import {HausApiClient, SortingEntityService} from "../../rest-api";
import {subscribeOnce} from "../../observable-extensions";
import {ActivatedRoute} from "@angular/router";
import {map, withLatestFrom} from "rxjs/operators";

@Injectable({
  providedIn: 'root'
})
export class DevicesService {
  private readonly _entityService = new SortingEntityService<DeviceModel>(d => d.name);

  get devices$(): Observable<DeviceModel[]> {
    return this._entityService.entitiesArray$;
  }

  get selectedDevice$(): Observable<DeviceModel | null> {
    return this._entityService.entitiesById$.pipe(
      withLatestFrom(this.route.paramMap),
      map(([devicesById, paramMap]) => {
        const deviceId = paramMap.has('deviceId') ? paramMap.get('deviceId') : null;
        return deviceId ? devicesById[deviceId] : null;
    }))
  }

  constructor(private readonly api: HausApiClient,
              private readonly route: ActivatedRoute) {
  }

  getAll() {
    return this._entityService.executeGetAll(() => this.api.getDevices());
  }

  turnOff(deviceId: number): Observable<void> {
    return subscribeOnce(this.api.turnDeviceOff(deviceId));
  }

  turnOn(deviceId: number): Observable<void> {
    return subscribeOnce(this.api.turnDeviceOn(deviceId));
  }
}
