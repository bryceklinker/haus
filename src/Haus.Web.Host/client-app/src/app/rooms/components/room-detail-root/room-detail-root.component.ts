import {Component} from "@angular/core";
import {RoomModel} from "../../models";
import {Observable} from "rxjs";
import {DeviceModel} from "../../../devices/models";
import {AppState} from "../../../app.state";
import {Store} from "@ngrx/store";
import {RoomsActions, selectRoomById} from "../../state";
import {ActivatedRoute} from "@angular/router";
import {map, mergeMap} from "rxjs/operators";
import {selectAllDevicesByRoomId} from "../../../devices/state";
import {RoomLightingChangeModel} from "../../models/room-lighting-change.model";

@Component({
  selector: 'room-detail-root',
  templateUrl: './room-detail-root.component.html',
  styleUrls: ['./room-detail-root.component.scss']
})
export class RoomDetailRootComponent {
  room$: Observable<RoomModel | undefined | null>;

  devices$: Observable<Array<DeviceModel>>;

  constructor(private readonly store: Store<AppState>,
              private readonly route: ActivatedRoute) {
    const roomId$ = this.route.paramMap.pipe(
      map(paramMap => paramMap.get('roomId') || '')
    );
    this.room$ = roomId$.pipe(
      mergeMap(roomId => this.store.select(selectRoomById(roomId)))
    )
    this.devices$ = roomId$.pipe(
      mergeMap(roomId => this.store.select(selectAllDevicesByRoomId(roomId)))
    )
  }

  onRoomLightingChanged($event: RoomLightingChangeModel) {
    this.store.dispatch(RoomsActions.changeRoomLighting.request($event));
  }
}
