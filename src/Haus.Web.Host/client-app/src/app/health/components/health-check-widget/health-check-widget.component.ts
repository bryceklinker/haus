import {Component, Input} from "@angular/core";

import {HausHealthCheckModel} from "../../../shared/models";

@Component({
  selector: 'health-check-widget',
  templateUrl: './health-check-widget.component.html',
  styleUrls: ['./health-check-widget.component.scss']
})
export class HealthCheckWidgetComponent {
  @Input() check: HausHealthCheckModel | undefined | null = null;

  get isError() {
    return this.check ? this.check.isError : false;
  }

  get isOk() {
    return this.check ? this.check.isOk : false;
  }

  get isWarning() {
    return this.check ? this.check.isWarn : false;
  }

  get name() {
    return this.check ? this.check.name : 'N/A';
  }

  get status() {
    return this.check ? this.check.status : 'N/A';
  }

  get description() {
    return this.check ? this.check.description : 'N/A';
  }

  get checkDuration() {
    return this.check ? `${this.check.durationOfCheckInSeconds} seconds` : 'N/A';
  }

  get exceptionMessage() {
    return this.check ? this.check.exceptionMessage : 'N/A';
  }

  get tags() {
    return this.check && this.check.tags ? this.check.tags.join(', ') : 'N/A';
  }
}
