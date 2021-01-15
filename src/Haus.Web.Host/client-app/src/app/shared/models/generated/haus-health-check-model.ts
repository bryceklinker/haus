import {HealthStatus} from './health-status';

export interface HausHealthCheckModel {
	name: string;
	status: HealthStatus;
	description: string;
	durationOfCheckInMilliseconds: number;
	tags: Array<string>;
	isOk: boolean;
	isWarn: boolean;
	isError: boolean;
	durationOfCheckInSeconds: number;
	exceptionMessage?: string;
}
