import {LightingState} from "./lighting-state.model";
import {LightingColorModel} from "./lighting-color.model";
import {LightingConstraintsModel} from "./lighting-constraints.model";

export interface LightingModel {
  state: LightingState;
  level: number;
  temperature: number;
  color: LightingColorModel;
  constraints: LightingConstraintsModel;
}


