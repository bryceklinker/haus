import {HealthStatus} from './health-status';
import {HausHealthCheckModel} from './haus-health-check-model';

export interface HausHealthReportModel {
	status: HealthStatus;
	durationOfCheckInMilliseconds: number;
	checks: Array<HausHealthCheckModel>;
	isOk: boolean;
	isDown: boolean;
	durationOfCheckInSeconds: number;
}
