import {Injectable} from "@angular/core";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {map, mergeMap} from "rxjs/operators";

import {DiscoveryActions} from "../state";
import {HausApiClient} from "../../rest-api";

@Injectable()
export class DiscoveryEffects {
  startDiscovery$ = createEffect(() => this.actions$.pipe(
    ofType(DiscoveryActions.startDiscovery.request),
    mergeMap(() => this.api.startDiscovery().pipe(
      map(() => DiscoveryActions.startDiscovery.success())
    ))
  ))

  stopDiscovery$ = createEffect(() => this.actions$.pipe(
    ofType(DiscoveryActions.stopDiscovery.request),
    mergeMap(() => this.api.stopDiscovery().pipe(
      map(() => DiscoveryActions.stopDiscovery.success())
    ))
  ))

  syncDiscovery$ = createEffect(() => this.actions$.pipe(
    ofType(DiscoveryActions.syncDiscovery.request),
    mergeMap(() => this.api.syncDiscovery().pipe(
      map(() => DiscoveryActions.syncDiscovery.success())
    ))
  ))

  getDiscovery$ = createEffect(() => this.actions$.pipe(
    ofType(DiscoveryActions.getDiscovery.request),
    mergeMap(() => this.api.getDiscovery().pipe(
      map(model => DiscoveryActions.getDiscovery.success(model))
    ))
  ))

  constructor(private readonly actions$: Actions,
              private readonly api: HausApiClient) {
  }
}
