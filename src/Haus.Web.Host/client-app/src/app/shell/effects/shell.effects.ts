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

  constructor(private actions$: Actions,
              private api: HausApiClient) {
  }
}
