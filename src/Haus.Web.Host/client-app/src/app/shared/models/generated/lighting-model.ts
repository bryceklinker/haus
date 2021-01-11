import {LightingState} from './lighting-state';
import {LevelLightingModel} from './level-lighting-model';
import {TemperatureLightingModel} from './temperature-lighting-model';
import {ColorLightingModel} from './color-lighting-model';

export interface LightingModel {
	state: LightingState;
	level: LevelLightingModel;
	temperature?: TemperatureLightingModel;
	color?: ColorLightingModel;
}
