import {Injectable} from "@angular/core";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {HausApiClient} from "../../shared/rest-api";
import {DeviceTypesActions, LightTypesActions} from "../state";
import {map, mergeMap} from "rxjs/operators";

@Injectable()
export class LightTypesEffects {
  loadLightTypes$ = createEffect(() => this.actions$.pipe(
    ofType(LightTypesActions.loadLightTypes.request),
    mergeMap(() => this.api.getLightTypes().pipe(
      map(result => LightTypesActions.loadLightTypes.success(result))
    ))
  ))

  constructor(private readonly actions$: Actions,
              private readonly api: HausApiClient) {
  }
}
