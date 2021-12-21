import {Component, EventEmitter, Input, Output} from '@angular/core';
import {DiagnosticsFilterParams} from '../../models';

@Component({
  selector: 'diagnostics-header',
  templateUrl: './diagnostics-header.component.html',
  styleUrls: ['./diagnostics-header.component.scss']
})
export class DiagnosticsHeaderComponent {
  @Input() isConnected: boolean | null = false;
  @Input() allowDiscovery: boolean | null = false;
  @Input() filterParams: DiagnosticsFilterParams | null = null;
  @Output() stopDiscovery = new EventEmitter<void>();
  @Output() startDiscovery = new EventEmitter<void>();
  @Output() syncDiscovery = new EventEmitter<void>();
  @Output() filterChange = new EventEmitter<DiagnosticsFilterParams>();

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

  get topic(): string {
    return this.filterParams?.topic ? this.filterParams.topic : '';
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

  onTopicChange($event: KeyboardEvent) {
    const target = $event.target as HTMLInputElement;
    this.filterChange.emit({...this.filterParams, topic: target.value});
  }
}
