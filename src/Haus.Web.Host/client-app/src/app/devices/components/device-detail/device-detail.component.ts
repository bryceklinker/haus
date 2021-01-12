import {Component, Input, Output, EventEmitter, OnChanges, SimpleChanges} from "@angular/core";
import {
  DeviceModel,
  DeviceType, LevelLightingModel, LightingConstraintsModel,
  LightingModel, LightType,
  MetadataModel, TemperatureLightingModel
} from "../../../shared/models";
import {DeviceLightingConstraintsModel} from "../../models";
import {toTitleCase} from "../../../shared/humanize";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";

@Component({
  selector: 'device-detail',
  templateUrl: './device-detail.component.html',
  styleUrls: ['./device-detail.component.scss']
})
export class DeviceDetailComponent implements OnChanges {
  @Input() device: DeviceModel | null | undefined = null;
  @Input() lightTypes: Array<LightType> | null | undefined = [];
  @Output() saveConstraints = new EventEmitter<DeviceLightingConstraintsModel>();
  @Output() saveDevice = new EventEmitter<DeviceModel>();

  form = new FormGroup({});

  get name(): string {
    return this.device ? this.device.name : 'N/A';
  }

  get externalId(): string {
    return this.device ? this.device.externalId : 'N/A';
  }

  get deviceType(): string {
    return this.device ? this.device.deviceType : 'N/A';
  }

  get lightType(): string {
    return this.device ? this.device.lightType : 'N/A';
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

  get levelLighting(): LevelLightingModel | null {
    return this.lighting ? this.lighting.level : null;
  }

  get temperatureLighting(): TemperatureLightingModel | null {
    return this.lighting && this.lighting.temperature ? this.lighting.temperature : null;
  }

  constructor(private readonly formBuilder: FormBuilder) {
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.form = this.formBuilder.group({
      name: [{value: this.name, disabled: this.device === null}, [Validators.required]],
      externalId: [{value: this.externalId, disabled: true}],
      deviceType: [{value: this.deviceType, disabled: true}],
      lightType: [{value: this.lightType, disabled: this.device === null}],
    })
  }

  humanize(lightType: LightType): string {
    return toTitleCase(lightType);
  }

  onSaveConstraints($event: LightingConstraintsModel) {
    if (!this.device) {
      return;
    }
    this.saveConstraints.emit({
      device: this.device,
      constraints: $event
    })
  }

  onSaveDevice() {
    if (!this.device) {
      return;
    }

    this.saveDevice.emit({
      ...this.device,
      ...this.form.getRawValue()
    });
  }
}
