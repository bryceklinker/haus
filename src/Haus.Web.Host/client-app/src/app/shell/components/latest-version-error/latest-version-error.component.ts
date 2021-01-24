import {Component, Input, Output, EventEmitter} from "@angular/core";

@Component({
  selector: 'latest-version-error',
  templateUrl: './latest-version-error.component.html',
  styleUrls: ['./latest-version-error.component.scss']
})
export class LatestVersionErrorComponent {
  @Input() error: any;
  @Output() retry = new EventEmitter<void>();

  get errorMessage(): string {
    return this.error ? this.error.message : 'N/A';
  }

  onRetry() {
    this.retry.emit();
  }
}
