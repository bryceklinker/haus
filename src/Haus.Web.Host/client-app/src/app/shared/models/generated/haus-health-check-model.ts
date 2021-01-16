import {HealthStatus} from './health-status';

export interface HausHealthCheckModel {
	name: string;
	status: HealthStatus;
	durationOfCheckInMilliseconds: number;
	tags: Array<string>;
	isOk: boolean;
	isWarn: boolean;
	isError: boolean;
	durationOfCheckInSeconds: number;
	description?: string;
	exceptionMessage?: string;
}
