import {Injectable} from "@angular/core";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {HttpClient} from "@angular/common/http";
import {DevicesActions} from "../actions";
import {catchError, map, mergeMap} from "rxjs/operators";
import {of} from "rxjs";
import {HausApiClient} from "../../shared/rest-api/haus-api-client";

@Injectable()
export class TurnDeviceOnEffects {
  turnOn$ = createEffect(() => this.actions$.pipe(
    ofType(DevicesActions.turnOn.request),
    mergeMap(({payload}) => this.hausApi.turnDeviceOn(payload).pipe(
      map(() => DevicesActions.turnOn.success(payload)),
      catchError(err => of(DevicesActions.turnOn.failed(payload, err)))
    ))
  ))

  constructor(private actions$: Actions, private hausApi: HausApiClient) {
  }
}
