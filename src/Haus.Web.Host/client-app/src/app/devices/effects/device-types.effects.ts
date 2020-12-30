import {Injectable} from "@angular/core";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {map, mergeMap} from "rxjs/operators";

import {HausApiClient} from "../../shared/rest-api";
import {DeviceTypesActions} from "../state";

@Injectable()
export class DeviceTypesEffects {
  loadDeviceTypes$ = createEffect(() => this.actions$.pipe(
    ofType(DeviceTypesActions.loadDeviceTypes.request),
    mergeMap(() => this.api.getDeviceTypes().pipe(
      map(result => DeviceTypesActions.loadDeviceTypes.success(result))
    ))
  ))

  constructor(private readonly actions$: Actions,
              private readonly api: HausApiClient) {
  }
}
