import {Injectable} from "@angular/core";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {catchError, map, mergeMap} from "rxjs/operators";
import {of} from "rxjs";

import {DevicesActions} from "../actions";
import {HausApiClient} from "../../shared/rest-api/haus-api-client";

@Injectable()
export class DiscoveryEffects {
  start$ = createEffect(() => this.actions$.pipe(
    ofType(DevicesActions.startDiscovery.request),
    mergeMap(() => this.hausApi.startDiscovery().pipe(
      map(() => DevicesActions.startDiscovery.success()),
      catchError(err => of(DevicesActions.startDiscovery.failed(err)))
    ))
  ))

  stop$ = createEffect(() => this.actions$.pipe(
    ofType(DevicesActions.stopDiscovery.request),
    mergeMap(() => this.hausApi.stopDiscovery().pipe(
      map(() => DevicesActions.stopDiscovery.success()),
      catchError(err => of(DevicesActions.stopDiscovery.failed(err)))
    ))
  ))

  sync$ = createEffect(() => this.actions$.pipe(
    ofType(DevicesActions.syncDiscovery.request),
    mergeMap(() => this.hausApi.syncDiscovery().pipe(
      map(() => DevicesActions.syncDiscovery.success()),
      catchError(err => of(DevicesActions.syncDiscovery.failed(err)))
    ))
  ))

  constructor(private actions$: Actions, private hausApi: HausApiClient) {
  }
}
