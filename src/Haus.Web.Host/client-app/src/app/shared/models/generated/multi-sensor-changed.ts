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
	hasTemperature: any;
	hasOccupancy: any;
	hasIlluminance: any;
	hasBattery: any;
	changes: Array<any>;
	hasMultipleChanges: any;
}
