import {Injectable, OnDestroy} from "@angular/core";
import {Observable} from "rxjs";
import {ActivatedRoute} from "@angular/router";
import {filter, map, takeUntil, withLatestFrom} from "rxjs/operators";

import {DeviceModel} from "../models";
import {HausApiClient, SortingEntityService} from "../../rest-api";
import {DestroyableSubject} from "../../destroyable-subject";

const DEVICE_ID_ROUTE_PARAM_NAME = 'deviceId';
@Injectable({
  providedIn: 'root'
})
export class DevicesService implements OnDestroy {
  private readonly entityService = new SortingEntityService<DeviceModel>(d => d.name);
  private readonly destroyable = new DestroyableSubject();

  get selectedDeviceId$(): Observable<string> {
    return this.destroyable.register(this.route.paramMap.pipe(
      map(paramMap => paramMap.get(DEVICE_ID_ROUTE_PARAM_NAME) || ''),
      filter(deviceId => deviceId !== '')
    ))
  }

  get devices$(): Observable<DeviceModel[]> {
    return this.destroyable.register(this.entityService.entitiesArray$);
  }

  get selectedDevice$(): Observable<DeviceModel | null> {
    return this.destroyable.register(this.entityService.entitiesById$.pipe(
      withLatestFrom(this.selectedDeviceId$),
      map(([devicesById, deviceId]) => {
        return deviceId ? devicesById[deviceId] : null;
      })
    ));
  }

  constructor(private readonly api: HausApiClient,
              private readonly route: ActivatedRoute) {
  }

  getAll() {
    return this.destroyable.register(this.entityService.executeGetAll(() => this.api.getDevices()));
  }

  turnOff(deviceId: number): Observable<void> {
    return this.destroyable.register(this.api.turnDeviceOff(deviceId));
  }

  turnOn(deviceId: number): Observable<void> {
    return this.destroyable.register(this.api.turnDeviceOn(deviceId));
  }

  ngOnDestroy(): void {
    this.entityService.destroy();
    this.destroyable.destroy();
  }
}
