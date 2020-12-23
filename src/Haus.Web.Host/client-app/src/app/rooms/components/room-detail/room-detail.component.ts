import {Component, Input} from '@angular/core';
import {RoomModel} from "../../../shared/rooms";


@Component({
  selector: 'room-detail',
  templateUrl: './room-detail.component.html',
  styleUrls: ['./room-detail.component.scss']
})
export class RoomDetailComponent {
  @Input() room: RoomModel | null = null;

  get name(): string {
    return this.room ? this.room.name : 'N/A';
  }
}
