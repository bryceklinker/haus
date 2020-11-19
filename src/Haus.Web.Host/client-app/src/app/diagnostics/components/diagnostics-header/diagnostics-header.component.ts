import {Component, Input} from "@angular/core";

@Component({
  selector: 'diagnostics-header',
  templateUrl: './diagnostics-header.component.html',
  styleUrls: ['./diagnostics-header.component.scss']
})
export class DiagnosticsHeaderComponent {
  @Input() isConnected: boolean = false;

  get status(): string {
    return this.isConnected ? 'connected' : 'disconnected';
  }

  get icon(): string {
    return this.isConnected ? 'sync' : 'sync_disabled';
  }
}
