import {LightingState} from "./lighting-state.model";
import {LightingColorModel} from "./lighting-color.model";

export interface LightingModel {
  state: LightingState;
  brightnessPercent: number;
  temperature: number;
  color: LightingColorModel;
}


