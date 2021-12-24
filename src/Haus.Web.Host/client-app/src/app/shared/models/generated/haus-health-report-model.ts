import {HealthStatus} from './health-status';
import {HausHealthCheckModel} from './haus-health-check-model';

export interface HausHealthReportModel {
	status: HealthStatus;
	durationOfCheckInMilliseconds: number;
	isOk: boolean;
	isWarn: boolean;
	isError: boolean;
	checks: Array<HausHealthCheckModel>;
	durationOfCheckInSeconds: number;
}
