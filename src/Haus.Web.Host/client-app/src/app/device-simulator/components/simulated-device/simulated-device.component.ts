import {Component, Input} from "@angular/core";
import {DeviceMetadataModel, DeviceModel} from "../../../shared/devices";

@Component({
  selector: 'simulated-device',
  templateUrl: './simulated-device.component.html',
  styleUrls: ['./simulated-device.component.scss']
})
export class SimulatedDeviceComponent{
  @Input() device: DeviceModel | null = null;

  get externalId(): string {
    return this.device ? this.device.externalId : 'N/A';
  }

  get deviceType(): string {
    return this.device ? this.device.deviceType : 'Unknown'
  }

  get metadata(): Array<DeviceMetadataModel> {
    return this.device ? this.device.metadata : [];
  }
}
