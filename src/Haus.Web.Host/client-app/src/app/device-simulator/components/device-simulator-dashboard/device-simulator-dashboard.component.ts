import {Component, Input, Output, EventEmitter} from "@angular/core";
import {SimulatedDeviceModel} from "../../../shared/models";

@Component({
  selector: 'device-simulator-dashboard',
  templateUrl: './device-simulator-dashboard.component.html',
  styleUrls: ['./device-simulator-dashboard.component.scss']
})
export class DeviceSimulatorDashboardComponent {
  @Input() simulatedDevices: Array<SimulatedDeviceModel> | undefined | null = null;
  @Input() isConnected: boolean | null | undefined;
  @Output() occupancyChange = new EventEmitter<SimulatedDeviceModel>();

  get status() {
    return this.isConnected ? 'connected' : 'disconnected';
  }

  onOccupancyChange(model: SimulatedDeviceModel) {
    this.occupancyChange.emit(model);
  }

  trackDevicesBy(index: number, device: SimulatedDeviceModel): string {
    return device.id;
  }
}
