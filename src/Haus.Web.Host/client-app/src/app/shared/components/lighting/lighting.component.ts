import {Component, EventEmitter, Input, Output} from "@angular/core";
import {
  ColorLightingModel,
  LevelLightingModel,
  LightingModel,
  LightingState,
  TemperatureLightingModel
} from "../../models";
import {MatSliderChange} from "@angular/material/slider";
import {MatSlideToggleChange} from "@angular/material/slide-toggle";

@Component({
  selector: 'lighting',
  templateUrl: './lighting.component.html',
  styleUrls: ['./lighting.component.scss']
})
export class LightingComponent {
  @Input() lighting: LightingModel | undefined | null = null;
  @Input() readonly: boolean = false;
  @Output() change = new EventEmitter<LightingModel>();

  get hasLighting(): boolean {
    return !!this.lighting;
  }

  get levelLighting(): LevelLightingModel | null {
    return this.lighting?.level ? this.lighting.level : null;
  }

  get lightingState(): LightingState | null {
    return this.lighting?.state ? this.lighting.state : null;
  }

  get temperatureLighting(): TemperatureLightingModel | null {
    return this.lighting?.temperature ? this.lighting.temperature : null;
  }

  get state(): boolean {
    return this.lightingState === LightingState.On;
  }

  get level(): number {
    return this.levelLighting == null ? 0 : this.levelLighting.value;
  }

  get minLevel(): number {
    return this.levelLighting == null ? 0 : this.levelLighting.min;
  }

  get maxLevel(): number {
    return this.levelLighting == null ? 0 : this.levelLighting.max;
  }

  get temperature(): number {
    return this.temperatureLighting == null ? 0 : this.temperatureLighting.value;
  }

  get minTemperature(): number {
    return this.temperatureLighting == null ? 0 : this.temperatureLighting.min;
  }

  get maxTemperature(): number {
    return this.temperatureLighting == null ? 0 : this.temperatureLighting.max;
  }

  get color(): ColorLightingModel | null {
    return this.lighting?.color ? this.lighting.color : null;
  }

  onStateChange($event: MatSlideToggleChange) {
    this.onLightingChanged({state: $event.checked ? LightingState.On : LightingState.Off});
  }

  onLevelChanged($event: MatSliderChange) {
    if (!this.levelLighting) {
      return;
    }

    this.onLightingChanged({level: {...this.levelLighting, value: $event.value || 0}});
  }

  onTemperatureChanged($event: MatSliderChange) {
    if (!this.temperatureLighting) {
      return;
    }

    this.onLightingChanged({temperature: {...this.temperatureLighting, value: $event.value || 0}});
  }

  onLightingColorChanged(color: ColorLightingModel) {
    this.onLightingChanged({color});
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
}
