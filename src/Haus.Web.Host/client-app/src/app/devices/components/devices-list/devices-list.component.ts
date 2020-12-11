import {Component, EventEmitter, Input, Output} from "@angular/core";
import {DeviceModel} from "../../models";

@Component({
  selector: 'devices-list',
  templateUrl: './devices-list.component.html',
  styleUrls: ['./devices-list.component.scss']
})
export class DevicesListComponent {
  @Input() devices: Array<DeviceModel> = [];
  @Output() deviceSelected = new EventEmitter<number>();

  onDeviceSelected(device: DeviceModel) {
    this.deviceSelected.emit(device.id);
  }
}
