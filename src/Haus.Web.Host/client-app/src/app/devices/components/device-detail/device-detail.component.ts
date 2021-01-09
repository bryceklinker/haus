import {Component, Input, Output, EventEmitter} from "@angular/core";
import {
  DeviceLightingConstraintsChangedEvent,
  DeviceModel,
  DeviceType,
  LightingConstraintsModel,
  LightingModel,
  MetadataModel
} from "../../../shared/models";

@Component({
  selector: 'device-detail',
  templateUrl: './device-detail.component.html',
  styleUrls: ['./device-detail.component.scss']
})
export class DeviceDetailComponent {
  @Input() device: DeviceModel | null | undefined = null;
  @Output() saveLightingConstraints = new EventEmitter<DeviceLightingConstraintsChangedEvent>();

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

  get lightingConstraints(): LightingConstraintsModel | null {
    return this.lighting ? this.lighting.constraints : null;
  }

  get canEdit(): boolean {
    return !this.device;
  }

  onSaveLightingConstraints($event: LightingConstraintsModel) {
    if(!this.device) {
      return;
    }

    this.saveLightingConstraints.emit({
      device: this.device,
      constraints: $event
    })
  }
}
