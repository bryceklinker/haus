import {LightingModel} from './lighting-model';

export interface RoomModel {
	id: number;
	name: string;
	lighting: LightingModel;
}
