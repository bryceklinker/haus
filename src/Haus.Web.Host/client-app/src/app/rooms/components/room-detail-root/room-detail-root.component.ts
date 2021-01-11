import {Component, OnInit} from "@angular/core";
import {Observable} from "rxjs";
import {Store} from "@ngrx/store";
import {ActivatedRoute} from "@angular/router";
import {map, mergeMap} from "rxjs/operators";
import {MatDialog} from "@angular/material/dialog";

import {AppState} from "../../../app.state";
import {RoomsActions, selectRoomById} from "../../state";
import {DevicesActions, selectAllDevicesByRoomId} from "../../../devices/state";
import {AssignDevicesToRoomDialogComponent} from "../assign-devices-to-room-dialog/assign-devices-to-room-dialog.component";
import {DeviceModel, RoomLightingChangedEvent, RoomModel} from "../../../shared/models";

@Component({
  selector: 'room-detail-root',
  templateUrl: './room-detail-root.component.html',
  styleUrls: ['./room-detail-root.component.scss']
})
export class RoomDetailRootComponent implements OnInit {
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

  ngOnInit(): void {
    this.store.dispatch(DevicesActions.loadDevices.request());
  }

  onRoomLightingChanged($event: RoomLightingChangedEvent) {
    this.store.dispatch(RoomsActions.changeRoomLighting.request($event));
  }

  onAssignDevices(room: RoomModel) {
    this.dialog.open(AssignDevicesToRoomDialogComponent, {data: room});
  }
}
