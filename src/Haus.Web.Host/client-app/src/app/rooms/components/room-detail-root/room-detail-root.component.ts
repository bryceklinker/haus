import {Component} from "@angular/core";
import {Observable} from "rxjs";
import {Store} from "@ngrx/store";
import {ActivatedRoute} from "@angular/router";
import {map, mergeMap} from "rxjs/operators";

import {RoomModel, RoomLightingChangeModel} from "../../models";
import {DeviceModel} from "../../../devices/models";
import {AppState} from "../../../app.state";
import {RoomsActions, selectRoomById} from "../../state";
import {selectAllDevicesByRoomId} from "../../../devices/state";
import {MatDialog} from "@angular/material/dialog";
import {AssignDevicesToRoomDialogComponent} from "../assign-devices-to-room-dialog/assign-devices-to-room-dialog.component";

@Component({
  selector: 'room-detail-root',
  templateUrl: './room-detail-root.component.html',
  styleUrls: ['./room-detail-root.component.scss']
})
export class RoomDetailRootComponent {
  room$: Observable<RoomModel | undefined | null>;

  devices$: Observable<Array<DeviceModel>>;

  constructor(private readonly store: Store<AppState>,
              private readonly route: ActivatedRoute,
              private readonly dialog: MatDialog) {
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

  onAssignDevices(room: RoomModel) {
    this.dialog.open(AssignDevicesToRoomDialogComponent, {data: room});
  }
}
