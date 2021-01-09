import {Component, Input} from '@angular/core';
import {DeviceModel} from "../../../shared/models";
import {getDeviceDisplayText} from "../../../shared/humanize/get-device-display-text";

@Component({
  selector: 'discovery-unassigned-devices',
  templateUrl: './discovery-unassigned-devices.component.html',
  styleUrls: ['./discovery-unassigned-devices.component.scss']
})
export class DiscoveryUnassignedDevicesComponent {
  @Input() devices: Array<DeviceModel> | null = [];
  @Input() roomIds: Array<number> | null = [];

  get roomStringIds(): Array<string> {
    return this.roomIds ? this.roomIds.map(id => id.toString()) : [];
  }

  getDeviceText(device: DeviceModel) {
    return getDeviceDisplayText(device);
  }
}
