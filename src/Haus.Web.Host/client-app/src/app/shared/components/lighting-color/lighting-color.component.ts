import {Component, Input, Output, EventEmitter} from "@angular/core";
import {ColorLightingModel} from "../../models";
import {MatSliderChange} from "@angular/material/slider";
import {toHexFromRGB} from "../../color-converter";

const DEFAULT_LIGHTING_COLOR: ColorLightingModel = {red: 0, green: 0, blue: 0};
@Component({
  selector: 'lighting-color',
  templateUrl: './lighting-color.component.html',
  styleUrls: ['./lighting-color.component.scss']
})
export class LightingColorComponent {
  @Input() color: ColorLightingModel | null = null;
  @Input() readonly: boolean = false;
  @Output() colorChange = new EventEmitter<ColorLightingModel>();

  get colorSample(): string {
    return toHexFromRGB(this.red, this.green, this.blue);
  }

  get red(): number {
    return this.color ? this.color.red : DEFAULT_LIGHTING_COLOR.red;
  }

  get green(): number {
    return this.color ? this.color.green : DEFAULT_LIGHTING_COLOR.green;
  }

  get blue(): number {
    return this.color ? this.color.blue : DEFAULT_LIGHTING_COLOR.blue;
  }

  onRedChanged($event: MatSliderChange) {
    this.onColorChanged({red: $event.value || DEFAULT_LIGHTING_COLOR.red});
  }

  onGreenChanged($event: MatSliderChange) {
    this.onColorChanged({green: $event.value || DEFAULT_LIGHTING_COLOR.green});
  }

  onBlueChanged($event: MatSliderChange) {
    this.onColorChanged({blue: $event.value || DEFAULT_LIGHTING_COLOR.blue});
  }

  private onColorChanged(change: Partial<ColorLightingModel>) {
    this.colorChange.emit({
      ...(this.color || DEFAULT_LIGHTING_COLOR),
      ...change
    })
  }
}
