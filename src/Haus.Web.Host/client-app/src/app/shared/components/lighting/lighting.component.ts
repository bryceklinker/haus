import {Component, EventEmitter, Input, Output} from "@angular/core";
import {LightingColorModel, LightingModel, LightingState} from "../../models";
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
    return this.lightingState === LightingState.On;
  }

  get lightingState(): LightingState | null {
    return this.lighting?.state ? this.lighting.state : null;
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

  get color(): LightingColorModel | null {
    return this.lighting ? this.lighting.color : null;
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

  onLightingColorChanged(color: LightingColorModel) {
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
