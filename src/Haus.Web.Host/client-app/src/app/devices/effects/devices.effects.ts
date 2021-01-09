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

  changeDeviceLightingConstraints$ = createEffect(() => this.actions$.pipe(
    ofType(DevicesActions.changeDeviceLightingConstraints.request),
    mergeMap(({payload}) => this.api.changeDeviceLightingConstraints(payload.deviceId, payload.constraints).pipe(
      map(() => DevicesActions.changeDeviceLightingConstraints.success(payload))
    ))
  ))

  constructor(private readonly actions$: Actions, private readonly api: HausApiClient) {
  }
}
