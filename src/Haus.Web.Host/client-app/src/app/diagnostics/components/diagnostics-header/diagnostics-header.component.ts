import {Component, EventEmitter, Input, Output} from "@angular/core";

@Component({
  selector: 'diagnostics-header',
  templateUrl: './diagnostics-header.component.html',
  styleUrls: ['./diagnostics-header.component.scss']
})
export class DiagnosticsHeaderComponent {
  @Input() isConnected: boolean | null = false;
  @Input() allowDiscovery: boolean | null = false;
  @Output() stopDiscovery = new EventEmitter<void>();
  @Output() startDiscovery = new EventEmitter<void>();
  @Output() syncDiscovery = new EventEmitter<void>();

  get status(): string {
    return this.isConnected ? 'connected' : 'disconnected';
  }

  get icon(): string {
    return this.isConnected ? 'sync' : 'sync_disabled';
  }

  get canStartDiscovery(): boolean {
    return !!this.allowDiscovery;
  }

  get canStopDiscovery(): boolean {
    return !this.allowDiscovery;
  }

  onStartDiscovery(): void {
    this.startDiscovery.emit();
  }

  onStopDiscovery(): void {
    this.stopDiscovery.emit();
  }

  onSyncDiscovery(): void {
    this.syncDiscovery.emit();
  }
}
