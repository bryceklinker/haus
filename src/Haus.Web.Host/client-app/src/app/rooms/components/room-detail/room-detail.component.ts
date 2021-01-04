import {Component, Input, EventEmitter, Output} from '@angular/core';
import {DeviceModel, LightingModel, RoomModel, RoomLightingChangedEvent} from "../../../shared/models";

@Component({
  selector: 'room-detail',
  templateUrl: './room-detail.component.html',
  styleUrls: ['./room-detail.component.scss']
})
export class RoomDetailComponent {
  @Input() room: RoomModel | undefined | null = null;
  @Input() devices: Array<DeviceModel> | undefined | null = [];
  @Output() lightingChange = new EventEmitter<RoomLightingChangedEvent>();
  @Output() assignDevices = new EventEmitter<RoomModel>();

  get lighting(): LightingModel | null {
    return this.room && this.room.lighting ? this.room.lighting : null;
  }

  get name(): string {
    return this.room ? this.room.name : 'N/A';
  }

  onLightingChanged($event: LightingModel) {
    if (!this.room) {
      return;
    }

    this.lightingChange.emit({
      room: this.room,
      lighting: $event
    });
  }

  onAssignDevices() {
    if (this.room) {
      this.assignDevices.emit(this.room);
    }
  }
}
