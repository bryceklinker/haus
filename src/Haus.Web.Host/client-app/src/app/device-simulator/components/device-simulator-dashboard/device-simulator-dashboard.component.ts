import {Component, Input} from "@angular/core";
import {SimulatedDeviceModel} from "../../../shared/models";

@Component({
  selector: 'device-simulator-dashboard',
  templateUrl: './device-simulator-dashboard.component.html',
  styleUrls: ['./device-simulator-dashboard.component.scss']
})
export class DeviceSimulatorDashboardComponent {
  @Input() simulatedDevices: Array<SimulatedDeviceModel> | undefined | null = null;
  @Input() isConnected: boolean | null | undefined;

  get status() {
    return this.isConnected ? 'connected' : 'disconnected';
  }
}
