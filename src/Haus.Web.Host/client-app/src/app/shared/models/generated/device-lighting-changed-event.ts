import {DeviceModel} from './device-model';
import {LightingModel} from './lighting-model';

export interface DeviceLightingChangedEvent {
	device: DeviceModel;
	lighting: LightingModel;
}
