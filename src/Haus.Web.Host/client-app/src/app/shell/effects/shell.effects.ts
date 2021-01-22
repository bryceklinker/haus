import {Injectable} from "@angular/core";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {HausApiClient} from "../../shared/rest-api";
import {ShellActions} from "../state";
import {map, mergeMap} from "rxjs/operators";

@Injectable()
export class ShellEffects {
  loadLatestVersion$ = createEffect(() => this.actions$.pipe(
    ofType(ShellActions.loadLatestVersion.request),
    mergeMap(() => this.api.getLatestVersion().pipe(
      map(version => ShellActions.loadLatestVersion.success(version))
    ))
  ));

  loadLatestPackages$ = createEffect(() => this.actions$.pipe(
    ofType(ShellActions.loadLatestPackages.request),
    mergeMap(() => this.api.getLatestPackages().pipe(
      map(result => ShellActions.loadLatestPackages.success(result))
    ))
  ))

  downloadPackage$ = createEffect(() => this.actions$.pipe(
    ofType(ShellActions.downloadPackage.request),
    mergeMap(({payload}) => this.api.downloadPackage(payload).pipe(
      map(() => ShellActions.downloadPackage.success())
    ))
  ))

  constructor(private actions$: Actions,
              private api: HausApiClient) {
  }
}
