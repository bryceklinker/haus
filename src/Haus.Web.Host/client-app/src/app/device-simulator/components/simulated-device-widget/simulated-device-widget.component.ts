import {Component, Input, Output, EventEmitter} from "@angular/core";
import {toTitleCase} from "../../../shared/humanize";
import {DeviceType, LightingModel, MetadataModel, SimulatedDeviceModel} from "../../../shared/models";

@Component({
  selector: 'simulated-device-widget',
  templateUrl: './simulated-device-widget.component.html',
  styleUrls: ['./simulated-device-widget.component.scss']
})
export class SimulatedDeviceWidgetComponent {
  @Input() simulatedDevice: SimulatedDeviceModel | null = null;
  @Output() occupancyChange = new EventEmitter<SimulatedDeviceModel>();

  get id(): string {
    return this.simulatedDevice ? this.simulatedDevice.id  : 'N/A';
  }

  get isLight(): boolean {
    return this.deviceType === DeviceType.Light;
  }

  get isMotionSensor(): boolean {
    return !!this.simulatedDevice && this.simulatedDevice.deviceType === DeviceType.MotionSensor;
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

  get isOccupied(): boolean {
    return !!this.simulatedDevice && this.simulatedDevice.isOccupied;
  }

  onOccupancyChange() {
    if (!this.simulatedDevice) {
      return;
    }

    this.occupancyChange.emit(this.simulatedDevice);
  }
}
