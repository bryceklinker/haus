import {Component, Input} from "@angular/core";
import {LogEntryModel} from "../../../shared/models";
import {convertLevelToIcon} from "../../../shared/icon-converter";

@Component({
  selector: 'recent-logs',
  templateUrl: './recent-logs.component.html',
  styleUrls: ['./recent-logs.component.scss']
})
export class RecentLogsComponent {
  @Input() logs: Array<LogEntryModel> | null = []

  get isLoading(): boolean {
    return !this.hasLogs;
  }

  get hasLogs(): boolean {
    return !!this.logs && this.logs.length > 0;
  }

  getIcon(log: LogEntryModel): string {
    return convertLevelToIcon(log.level);
  }

  trackByTimestamp(index: number, log: LogEntryModel): string {
    return log.timestamp;
  }
}
