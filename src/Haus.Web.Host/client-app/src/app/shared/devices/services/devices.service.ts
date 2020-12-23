import {Injectable, OnDestroy} from "@angular/core";
import {Observable, Subject} from "rxjs";
import {ActivatedRoute} from "@angular/router";
import {map, takeUntil, withLatestFrom} from "rxjs/operators";

import {DeviceModel} from "../models";
import {HausApiClient, SortingEntityService} from "../../rest-api";
import {subscribeOnce} from "../../observable-extensions";

@Injectable({
  providedIn: 'root'
})
export class DevicesService implements OnDestroy {
  private readonly entityService = new SortingEntityService<DeviceModel>(d => d.name);
  private readonly unsubscribe$ = new Subject();

  get devices$(): Observable<DeviceModel[]> {
    return this.entityService.entitiesArray$;
  }

  get selectedDevice$(): Observable<DeviceModel | null> {
    return this.entityService.entitiesById$.pipe(
      withLatestFrom(this.route.paramMap),
      map(([devicesById, paramMap]) => {
        const deviceId = paramMap.has('deviceId') ? paramMap.get('deviceId') : null;
        return deviceId ? devicesById[deviceId] : null;
      }),
      takeUntil(this.unsubscribe$)
    )
  }

  constructor(private readonly api: HausApiClient,
              private readonly route: ActivatedRoute) {
  }

  getAll() {
    return this.entityService.executeGetAll(() => this.api.getDevices());
  }

  turnOff(deviceId: number): Observable<void> {
    return subscribeOnce(this.api.turnDeviceOff(deviceId));
  }

  turnOn(deviceId: number): Observable<void> {
    return subscribeOnce(this.api.turnDeviceOn(deviceId));
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
  }
}
