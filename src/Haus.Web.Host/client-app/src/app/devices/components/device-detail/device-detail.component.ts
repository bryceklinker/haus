import {Component, Input} from "@angular/core";
import {DeviceModel, DeviceType, LightingModel, MetadataModel} from "../../../shared/models";

@Component({
  selector: 'device-detail',
  templateUrl: './device-detail.component.html',
  styleUrls: ['./device-detail.component.scss']
})
export class DeviceDetailComponent {
  @Input() device: DeviceModel | null | undefined = null;

  get name(): string {
    return this.device ? this.device.name : 'N/A';
  }

  get externalId(): string {
    return this.device ? this.device.externalId : 'N/A';
  }

  get type(): string {
    return this.device ? this.device.deviceType : 'N/A';
  }

  get isLight(): boolean {
    return this.device?.deviceType === DeviceType.Light;
  }

  get metadata(): Array<MetadataModel> {
    return this.device && this.device.metadata ? this.device.metadata : [];
  }

  get lighting(): LightingModel | null {
    return this.device && this.device.lighting ? this.device.lighting : null;
  }

  get canEdit(): boolean {
    return !this.device;
  }
}
