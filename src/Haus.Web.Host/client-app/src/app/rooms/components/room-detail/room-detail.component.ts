import {Component, Input, EventEmitter, Output} from '@angular/core';
import {RoomModel} from "../../models";
import {DeviceModel} from "../../../devices/models";
import {LightingModel} from "../../../shared/models";
import {RoomLightingChangeModel} from "../../models/room-lighting-change.model";


@Component({
  selector: 'room-detail',
  templateUrl: './room-detail.component.html',
  styleUrls: ['./room-detail.component.scss']
})
export class RoomDetailComponent {
  @Input() room: RoomModel | undefined | null = null;
  @Input() devices: Array<DeviceModel> | undefined | null = [];
  @Output() lightingChange = new EventEmitter<RoomLightingChangeModel>();

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
      roomId: this.room.id,
      lighting: $event
    });
  }
}
