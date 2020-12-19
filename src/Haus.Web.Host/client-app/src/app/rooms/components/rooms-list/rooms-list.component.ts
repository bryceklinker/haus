import {Component, Input, Output, EventEmitter} from "@angular/core";
import {RoomModel} from "../../models/room.model";

@Component({
  selector: 'rooms-list',
  templateUrl: './rooms-list.component.html',
  styleUrls: ['./rooms-list.component.scss']
})
export class RoomsListComponent {
  @Input() rooms: Array<RoomModel> | null = [];
  @Output() addRoom = new EventEmitter<void>();

  onAddRoom(): void {
    this.addRoom.emit();
  }
}
