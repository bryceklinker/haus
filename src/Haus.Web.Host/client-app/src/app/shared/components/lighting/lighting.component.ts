import {Component, EventEmitter, Input, Output} from "@angular/core";
import {LightingColorModel, LightingModel, LightingState} from "../../models";
import {MatSliderChange} from "@angular/material/slider";
import {MatSlideToggleChange} from "@angular/material/slide-toggle";

const DEFAULT_LIGHTING_COLOR: LightingColorModel = {red: 0, green: 0, blue: 0};
@Component({
  selector: 'lighting',
  templateUrl: './lighting.component.html',
  styleUrls: ['./lighting.component.scss']
})
export class LightingComponent {
  @Input() lighting: LightingModel | undefined | null = null;
  @Output() change = new EventEmitter<LightingModel>();

  get colorSample(): string {
    return `#${this.convertByteToHex(this.red)}${this.convertByteToHex(this.green)}${this.convertByteToHex(this.blue)}`;
  }

  get hasLighting(): boolean {
    return !!this.lighting;
  }

  get level(): number {
    return this.lighting ? this.lighting.level : 0;
  }

  get minLevel(): number {
    return this.lighting?.constraints ? this.lighting.constraints.minLevel : 0;
  }

  get maxLevel(): number {
    return this.lighting?.constraints ? this.lighting.constraints.maxLevel : 0;
  }

  get state(): boolean {
    return !!this.lighting && this.lighting.state === LightingState.On;
  }

  get temperature(): number {
    return this.lighting ? this.lighting.temperature : 0;
  }

  get minTemperature(): number {
    return this.lighting?.constraints ? this.lighting.constraints.minTemperature : 0;
  }

  get maxTemperature(): number {
    return this.lighting?.constraints ? this.lighting.constraints.maxTemperature : 0;
  }

  get red(): number {
    return this.lighting && this.lighting.color ? this.lighting.color.red : 0;
  }

  get green(): number {
    return this.lighting && this.lighting.color ? this.lighting.color.green : 0;
  }

  get blue(): number {
    return this.lighting && this.lighting.color ? this.lighting.color.blue : 0;
  }

  onLevelChanged(change: MatSliderChange) {
    this.onLightingChanged({level: change.value || 0});
  }

  onStateChange($event: MatSlideToggleChange) {
    this.onLightingChanged({state: $event.checked ? LightingState.On : LightingState.Off});
  }

  onTemperatureChanged($event: MatSliderChange) {
    this.onLightingChanged({temperature: $event.value || 0});
  }

  onRedChanged($event: MatSliderChange) {
    this.onLightingColorChanged({red: $event.value || 0});
  }

  onGreenChanged($event: MatSliderChange) {
    this.onLightingColorChanged({green: $event.value || 0});
  }

  onBlueChanged($event: MatSliderChange) {
    this.onLightingColorChanged({blue: $event.value || 0});
  }

  onLightingColorChanged(color: Partial<LightingColorModel>) {
    this.onLightingChanged({
      ...this.lighting,
      color: {
        ...(this.lighting && this.lighting.color ? this.lighting.color : DEFAULT_LIGHTING_COLOR),
        ...color
      }
    })
  }

  onLightingChanged(lighting: Partial<LightingModel>) {
    if (!this.lighting) {
      return;
    }

    this.change.emit({
      ...this.lighting,
      ...lighting
    });
  }

  convertByteToHex(byte: number): string {
    const hex = byte.toString(16);
    return hex.length == 1 ? `0${hex}` : hex;
  }
}
