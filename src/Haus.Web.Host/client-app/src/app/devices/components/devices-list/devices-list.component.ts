import {Component, Input} from "@angular/core";
import {DeviceModel} from "../../../shared/devices";

@Component({
  selector: 'devices-list',
  templateUrl: './devices-list.component.html',
  styleUrls: ['./devices-list.component.scss']
})
export class DevicesListComponent {
  @Input() devices: Array<DeviceModel> | null = [];
}
