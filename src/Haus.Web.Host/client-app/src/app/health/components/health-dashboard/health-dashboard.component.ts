import {Component, Input} from "@angular/core";
import {HausHealthCheckModel, HausHealthReportModel} from "../../../shared/models";

@Component({
  selector: 'health-dashboard',
  templateUrl: './health-dashboard.component.html',
  styleUrls: ['./health-dashboard.component.scss']
})
export class HealthDashboardComponent {
  @Input() report: HausHealthReportModel | undefined | null = null;

  get isError(): boolean {
    return this.report ? this.report.isError : false;
  }

  get isWarning(): boolean {
    return this.report ? this.report.isWarn : false;
  }

  get isOk(): boolean {
    return this.report ? this.report.isOk : false;
  }

  get hasReport(): boolean {
    return !!this.report;
  }

  get checks(): HausHealthCheckModel[] {
    return this.report ? this.report.checks : [];
  }

  trackChecksBy(index: number, check: HausHealthCheckModel): string {
    return check.name;
  }
}
