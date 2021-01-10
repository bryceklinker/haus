import {Component, Input} from "@angular/core";
import {toTitleCase} from "../../../shared/humanize";
import {DeviceType, LightingModel, MetadataModel, SimulatedDeviceModel} from "../../../shared/models";

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

  get isLight(): boolean {
    return this.deviceType === DeviceType.Light;
  }

  get deviceType(): string {
    return this.simulatedDevice ? toTitleCase(this.simulatedDevice.deviceType) : 'N/A';
  }

  get lighting(): LightingModel | null {
    return this.simulatedDevice && this.simulatedDevice.lighting ? this.simulatedDevice.lighting : null;
  }

  get metadata(): Array<MetadataModel> {
    return this.simulatedDevice ? this.simulatedDevice.metadata : [];
  }
}
