import {OccupancyChangedModel} from './occupancy-changed-model';
import {TemperatureChangedModel} from './temperature-changed-model';
import {IlluminanceChangedModel} from './illuminance-changed-model';
import {BatteryChangedModel} from './battery-changed-model';

export interface MultiSensorChanged {
	deviceId: string;
	occupancyChanged: OccupancyChangedModel;
	temperatureChanged: TemperatureChangedModel;
	illuminanceChanged: IlluminanceChangedModel;
	batteryChanged: BatteryChangedModel;
	hasTemperature: boolean;
	hasOccupancy: boolean;
	hasIlluminance: boolean;
	hasBattery: boolean;
	changes: Array<boolean>;
	hasMultipleChanges: boolean;
}
