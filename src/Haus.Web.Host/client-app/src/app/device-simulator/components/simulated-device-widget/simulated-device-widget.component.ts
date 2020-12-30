import {Component, Input} from "@angular/core";
import {SimulatedDeviceModel} from "../../models";
import {MetadataModel} from "../../../shared/models/metadata.model";

@Component({
  selector: 'simulated-device-widget',
  templateUrl: './simulated-device-widget.component.html',
  styleUrls: ['./simulated-device-widget.component.scss']
})
export class SimulatedDeviceWidgetComponent {
  @Input() simulatedDevice: SimulatedDeviceModel | null = null;

  get id(): string {
    return this.simulatedDevice ? this.simulatedDevice.id  : 'N/A';
  }

  get deviceType(): string {
    return this.simulatedDevice ? this.simulatedDevice.deviceType : 'N/A';
  }

  get metadata(): Array<MetadataModel> {
    return this.simulatedDevice ? this.simulatedDevice.metadata : [];
  }
}
