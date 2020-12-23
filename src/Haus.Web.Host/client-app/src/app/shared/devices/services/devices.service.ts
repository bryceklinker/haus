import {Injectable, OnDestroy} from "@angular/core";
import {Observable} from "rxjs";
import {ActivatedRoute} from "@angular/router";
import {map, takeUntil, withLatestFrom} from "rxjs/operators";

import {DeviceModel} from "../models";
import {HausApiClient, SortingEntityService} from "../../rest-api";
import {DestroyableSubject} from "../../destroyable-subject";

@Injectable({
  providedIn: 'root'
})
export class DevicesService implements OnDestroy {
  private readonly entityService = new SortingEntityService<DeviceModel>(d => d.name);
  private readonly destroyable = new DestroyableSubject();

  get devices$(): Observable<DeviceModel[]> {
    return this.destroyable.register(this.entityService.entitiesArray$);
  }

  get selectedDevice$(): Observable<DeviceModel | null> {
    return this.destroyable.register(this.entityService.entitiesById$.pipe(
      withLatestFrom(this.route.paramMap),
      map(([devicesById, paramMap]) => {
        const deviceId = paramMap.has('deviceId') ? paramMap.get('deviceId') : null;
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
