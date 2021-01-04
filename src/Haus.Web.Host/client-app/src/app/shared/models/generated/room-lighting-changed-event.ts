import {RoomModel} from './room-model';
import {LightingModel} from './lighting-model';

export interface RoomLightingChangedEvent {
	room: RoomModel;
	lighting: LightingModel;
}
