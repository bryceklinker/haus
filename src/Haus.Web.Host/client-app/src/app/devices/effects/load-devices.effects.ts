import {Injectable} from "@angular/core";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {DevicesActions} from "../actions";
import {catchError, map, mergeMap} from "rxjs/operators";
import {of} from "rxjs";
import {HausApiClient} from "../../shared/rest-api/haus-api-client";

@Injectable()
export class LoadDevicesEffects {
  load$ = createEffect(() => this.actions$.pipe(
    ofType(DevicesActions.load.request),
    mergeMap(() => this.hausApi.getDevices().pipe(
      map(result => DevicesActions.load.success(result)),
      catchError(err => of(DevicesActions.load.failed(err)))
    ))
  ))

  constructor(private actions$: Actions, private hausApi: HausApiClient) {
  }
}
