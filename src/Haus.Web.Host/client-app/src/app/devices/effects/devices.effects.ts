import {Injectable} from "@angular/core";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {of} from "rxjs";
import {catchError, map, mergeMap} from "rxjs/operators";

import {DevicesActions} from "../state";
import {HausApiClient} from "../../shared/rest-api";

@Injectable()
export class DevicesEffects {
  loadDevices$ = createEffect(() => this.actions$.pipe(
    ofType(DevicesActions.loadDevices.request),
    mergeMap(() => this.api.getDevices().pipe(
      map(result => DevicesActions.loadDevices.success(result)),
      catchError(err => of(DevicesActions.loadDevices.failed(err)))
    ))
  ))

  turnOnDevice$ = createEffect(() => this.actions$.pipe(
    ofType(DevicesActions.turnOnDevice.request),
    mergeMap(({payload}) => this.api.turnDeviceOn(payload).pipe(
      map(() => DevicesActions.turnOnDevice.success(payload)),
      catchError(err => of(DevicesActions.turnOnDevice.failed(payload, err)))
    ))
  ))

  turnOffDevice$ = createEffect(() => this.actions$.pipe(
    ofType(DevicesActions.turnOffDevice.request),
    mergeMap(({payload}) => this.api.turnDeviceOff(payload).pipe(
      map(() => DevicesActions.turnOffDevice.success(payload)),
      catchError(err => of(DevicesActions.turnOffDevice.failed(payload, err)))
    ))
  ))

  startDiscovery$ = createEffect(() => this.actions$.pipe(
    ofType(DevicesActions.startDiscovery.request),
    mergeMap(() => this.api.startDiscovery().pipe(
      map(() => DevicesActions.startDiscovery.success())
    ))
  ))

  stopDiscovery$ = createEffect(() => this.actions$.pipe(
    ofType(DevicesActions.stopDiscovery.request),
    mergeMap(() => this.api.stopDiscovery().pipe(
      map(() => DevicesActions.stopDiscovery.success())
    ))
  ))

  syncDiscovery$ = createEffect(() => this.actions$.pipe(
    ofType(DevicesActions.syncDiscovery.request),
    mergeMap(() => this.api.syncDiscovery().pipe(
      map(() => DevicesActions.syncDiscovery.success())
    ))
  ))

  constructor(private readonly actions$: Actions, private readonly api: HausApiClient) {
  }
}
