import {Component, Input} from "@angular/core";

import {DeviceModel} from "../../../shared/devices";

@Component({
  selector: 'device-simulator-dashboard',
  templateUrl: './device-simulator-dashboard.component.html',
  styleUrls: ['./device-simulator-dashboard.component.scss']
})
export class DeviceSimulatorDashboardComponent {
  @Input() devices: DeviceModel[] = [];
}
