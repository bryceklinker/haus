import {Injectable} from "@angular/core";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {DevicesActions} from "../actions";
import {catchError, map, mergeMap} from "rxjs/operators";
import {HttpClient} from "@angular/common/http";
import {DeviceModel} from "../models";
import {ListResult} from "../../shared/models";
import {of} from "rxjs";

@Injectable()
export class LoadDevicesEffects {
  load$ = createEffect(() => this.actions$.pipe(
    ofType(DevicesActions.load.request),
    mergeMap(() => this.http.get<ListResult<DeviceModel>>('/api/devices').pipe(
      map(result => DevicesActions.load.success(result)),
      catchError(err => of(DevicesActions.load.failed(err)))
    ))
  ))

  constructor(private actions$: Actions, private http: HttpClient) {
  }
}
