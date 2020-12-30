import {Injectable} from "@angular/core";
import {Actions, createEffect, ofType} from "@ngrx/effects";
import {HausApiClient} from "../../shared/rest-api";
import {RoomsActions} from "../state";
import {debounceTime, map, mergeMap} from "rxjs/operators";

@Injectable()
export class RoomsEffects {
  loadRooms$ = createEffect(() => this.actions$.pipe(
    ofType(RoomsActions.loadRooms.request),
    mergeMap(() => this.api.getRooms().pipe(
      map(result => RoomsActions.loadRooms.success(result))
    ))
  ));

  addRoom$ = createEffect(() => this.actions$.pipe(
    ofType(RoomsActions.addRoom.request),
    mergeMap(({payload}) => this.api.addRoom(payload).pipe(
      map(result => RoomsActions.addRoom.success(result))
    ))
  ))

  changeLighting$ = createEffect(() => this.actions$.pipe(
    ofType(RoomsActions.changeRoomLighting.request),
    debounceTime(200),
    mergeMap(({payload}) => this.api.changeRoomLighting(payload.roomId, payload.lighting).pipe(
      map(() => RoomsActions.changeRoomLighting.success(payload))
    ))
  ))

  constructor(private readonly actions$: Actions,
              private readonly api: HausApiClient) {
  }
}
