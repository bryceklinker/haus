import {HealthStatus} from './health-status';

export interface HausHealthCheckModel {
	name: string;
	status: HealthStatus;
	description: string;
	durationOfCheckInMilliseconds: number;
	exceptionMessage: string;
	tags: Array<string>;
	isOk: boolean;
	isDown: boolean;
	durationOfCheckInSeconds: number;
}
