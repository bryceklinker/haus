import {Injectable} from '@angular/core';
import {Actions, createEffect, ofType} from '@ngrx/effects';
import {debounceTime, map, mergeMap} from 'rxjs/operators';

import {HausApiClient} from '../../shared/rest-api';
import {RoomsActions} from '../state';

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
  ));

  updateRoom$ = createEffect(() => this.actions$.pipe(
    ofType(RoomsActions.updateRoom.request),
    mergeMap(({payload}) => this.api.updateRoom(payload.id, payload).pipe(
      map(() => RoomsActions.updateRoom.success(payload))
    ))
  ));

  changeLighting$ = createEffect(() => this.actions$.pipe(
    ofType(RoomsActions.changeRoomLighting.request),
    debounceTime(200),
    mergeMap(({payload}) => this.api.changeRoomLighting(payload.room.id, payload.lighting).pipe(
      map(() => RoomsActions.changeRoomLighting.success(payload))
    ))
  ));

  assignDevices$ = createEffect(() => this.actions$.pipe(
    ofType(RoomsActions.assignDevicesToRoom.request),
    mergeMap(({payload}) => this.api.assignDevicesToRoom(payload.roomId, payload.deviceIds).pipe(
      map(() => RoomsActions.assignDevicesToRoom.success(payload))
    ))
  ));

  constructor(private readonly actions$: Actions,
              private readonly api: HausApiClient) {
  }
}
