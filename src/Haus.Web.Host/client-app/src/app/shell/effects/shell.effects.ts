import {Injectable} from "@angular/core";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {HausApiClient} from "../../shared/rest-api";
import {ShellActions} from "../state";
import {catchError, map, mergeMap} from "rxjs/operators";
import {of} from "rxjs";

@Injectable()
export class ShellEffects {
  loadLatestVersion$ = createEffect(() => this.actions$.pipe(
    ofType(ShellActions.loadLatestVersion.request),
    mergeMap(() => this.api.getLatestVersion().pipe(
      map(version => ShellActions.loadLatestVersion.success(version)),
      catchError(err => of(ShellActions.loadLatestVersion.failed(err)))
    ))
  ));

  loadLatestPackages$ = createEffect(() => this.actions$.pipe(
    ofType(ShellActions.loadLatestPackages.request),
    mergeMap(() => this.api.getLatestPackages().pipe(
      map(result => ShellActions.loadLatestPackages.success(result)),
      catchError(err => of(ShellActions.loadLatestPackages.failed(err)))
    ))
  ))

  downloadPackage$ = createEffect(() => this.actions$.pipe(
    ofType(ShellActions.downloadPackage.request),
    mergeMap(({payload}) => this.api.downloadPackage(payload).pipe(
      map(() => ShellActions.downloadPackage.success()),
      catchError(err => of(ShellActions.downloadPackage.failed(err)))
    ))
  ));

  constructor(private actions$: Actions,
              private api: HausApiClient) {

  }
}
