import {Injectable} from "@angular/core";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {DevicesActions} from "../actions";
import {catchError, map, mergeMap} from "rxjs/operators";
import {of} from "rxjs";
import {HausApiClient} from "../../shared/rest-api/haus-api-client";

@Injectable()
export class TurnDeviceOffEffects {
  turnOff$ = createEffect(() => this.actions$.pipe(
    ofType(DevicesActions.turnOff.request),
    mergeMap(({payload}) => this.hausApi.turnDeviceOff(payload).pipe(
      map(() => DevicesActions.turnOff.success(payload)),
      catchError(err => of(DevicesActions.turnOff.failed(payload, err)))
    ))
  ))

  constructor(private actions$: Actions, private hausApi: HausApiClient) {
  }
}
