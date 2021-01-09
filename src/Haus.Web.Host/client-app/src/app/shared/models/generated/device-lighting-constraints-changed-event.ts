import {DeviceModel} from './device-model';
import {LightingConstraintsModel} from './lighting-constraints-model';

export interface DeviceLightingConstraintsChangedEvent {
	device: DeviceModel;
	constraints: LightingConstraintsModel;
}
