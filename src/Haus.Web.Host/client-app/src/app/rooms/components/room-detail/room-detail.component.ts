import {Component, Input} from '@angular/core';
import {RoomModel} from "../../models";
import {DeviceModel} from "../../../devices/models";


@Component({
  selector: 'room-detail',
  templateUrl: './room-detail.component.html',
  styleUrls: ['./room-detail.component.scss']
})
export class RoomDetailComponent {
  @Input() room: RoomModel | undefined | null = null;
  @Input() devices: Array<DeviceModel> | undefined | null = [];

  get name(): string {
    return this.room ? this.room.name : 'N/A';
  }
}
