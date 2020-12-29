import {Component, Input} from '@angular/core';
import {RoomModel} from "../../models";
import {DeviceModel} from "../../../devices/models";


@Component({
  selector: 'room-detail',
  templateUrl: './room-detail.component.html',
  styleUrls: ['./room-detail.component.scss']
})
export class RoomDetailComponent {
  @Input() room: RoomModel | null = null;
  @Input() devices: Array<DeviceModel> = [];

  get name(): string {
    return this.room ? this.room.name : 'N/A';
  }
}
