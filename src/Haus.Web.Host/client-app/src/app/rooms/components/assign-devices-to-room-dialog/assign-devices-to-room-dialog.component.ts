import {Component, Inject, OnDestroy, OnInit} from "@angular/core";
import {Observable} from "rxjs";
import {Store} from "@ngrx/store";

import {AppState} from "../../../app.state";
import {selectUnassignedDevices} from "../../../devices/state";
import {toTitleCase} from "../../../shared/humanize";
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {RoomsActions} from "../../state";
import {Actions, ofType} from "@ngrx/effects";
import {DestroyableSubject} from "../../../shared/destroyable-subject";
import {DeviceModel, RoomModel} from "../../../shared/models";

@Component({
  selector: 'assign-devices-to-room-dialog',
  templateUrl: './assign-devices-to-room-dialog.component.html',
  styleUrls: ['./assign-devices-to-room-dialog.component.scss']
})
export class AssignDevicesToRoomDialogComponent implements OnInit, OnDestroy {
  private readonly destroyable = new DestroyableSubject();
  unassignedDevices$: Observable<Array<DeviceModel>>;
  private devicesToAssign: Array<DeviceModel> = [];

  get hasEmptySelection(): boolean {
    return this.devicesToAssign.length <= 0;
  }

  constructor(private readonly store: Store<AppState>,
              private readonly actions: Actions,
              private readonly dialogRef: MatDialogRef<AssignDevicesToRoomDialogComponent>,
              @Inject(MAT_DIALOG_DATA) private readonly room: RoomModel) {

    this.unassignedDevices$ = store.select(selectUnassignedDevices);
  }

  onAssignDevicesToRoom() {
    this.store.dispatch(RoomsActions.assignDevicesToRoom.request({
      roomId: this.room.id,
      deviceIds: this.devicesToAssign.map(d => d.id)
    }))
  }

  ngOnInit(): void {
    this.devicesToAssign = [];
    this.actions.pipe(
      ofType(RoomsActions.assignDevicesToRoom.success)
    ).subscribe(() => this.dialogRef.close());
  }

  ngOnDestroy(): void {
    this.destroyable.destroy();
  }

  onCancelAssignDevices() {
    this.dialogRef.close();
  }

  humanize(deviceType: string) {
    return toTitleCase(deviceType);
  }

  onChecked(device: DeviceModel) {
    if (this.devicesToAssign.includes(device)) {
      this.devicesToAssign = [...this.devicesToAssign.filter(d => d.id !== device.id)];
    } else {
      this.devicesToAssign = [...this.devicesToAssign, device];
    }
  }
}
