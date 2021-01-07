import {Component, Input, EventEmitter, Output} from "@angular/core";
import {DeviceModel, DevicesAssignedToRoomEvent, RoomModel} from "../../../shared/models";
import {getDeviceDisplayText} from "../../../shared/humanize/get-device-display-text";
import {CdkDragDrop} from "@angular/cdk/drag-drop";

@Component({
  selector: 'discovery-room',
  templateUrl: './discovery-room.component.html',
  styleUrls: ['./discovery-room.component.scss']
})
export class DiscoveryRoomComponent {
  @Input() room: RoomModel | null = null;
  @Input() devices: Array<DeviceModel> | null = [];
  @Output() assignDevice = new EventEmitter<DevicesAssignedToRoomEvent>();

  get roomId(): number {
    return this.room ? this.room.id : -1;
  }

  get name(): string {
    return this.room ? this.room.name : 'N/A';
  }

  get devicesInRoom(): Array<DeviceModel> {
    return this.devices ? this.devices.filter(d => d.roomId === this.roomId) : [];
  }

  getDeviceText(device: DeviceModel): string {
    return getDeviceDisplayText(device);
  }

  onDeviceAssigned($event: CdkDragDrop<DeviceModel>) {
    this.assignDevice.emit({roomId: this.roomId, deviceIds: [$event.item.data.id]});
  }
}
