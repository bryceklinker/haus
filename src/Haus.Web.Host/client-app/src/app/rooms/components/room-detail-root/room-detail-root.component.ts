import {Component} from "@angular/core";
import {RoomModel, RoomsService} from "../../../shared/rooms";
import {Observable} from "rxjs";
import {DeviceModel} from "../../../shared/devices";
import {tap} from "rxjs/operators";

@Component({
  selector: 'room-detail-root',
  templateUrl: './room-detail-root.component.html',
  styleUrls: ['./room-detail-root.component.scss']
})
export class RoomDetailRootComponent {
  get room$(): Observable<RoomModel | null> {
    return this.service.selectedRoom$;
  }

  get devices$(): Observable<Array<DeviceModel>> {
    return this.service.selectedRoomDevices$;
  }

  constructor(private readonly service: RoomsService) {
  }
}
