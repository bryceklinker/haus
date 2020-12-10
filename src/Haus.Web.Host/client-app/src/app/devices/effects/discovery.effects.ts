import {Injectable} from "@angular/core";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {HttpClient} from "@angular/common/http";
import {catchError, map, mergeMap} from "rxjs/operators";
import {of} from "rxjs";

import {DevicesActions} from "../actions";

@Injectable()
export class DiscoveryEffects {
  start$ = createEffect(() => this.actions$.pipe(
    ofType(DevicesActions.startDiscovery.request),
    mergeMap(() => this.http.post('/api/devices/start-discovery', null).pipe(
      map(() => DevicesActions.startDiscovery.success()),
      catchError(err => of(DevicesActions.startDiscovery.failed(err)))
    ))
  ))

  stop$ = createEffect(() => this.actions$.pipe(
    ofType(DevicesActions.stopDiscovery.request),
    mergeMap(() => this.http.post('/api/devices/stop-discovery', null).pipe(
      map(() => DevicesActions.stopDiscovery.success()),
      catchError(err => of(DevicesActions.stopDiscovery.failed(err)))
    ))
  ))

  constructor(private actions$: Actions, private http: HttpClient) {
  }
}
