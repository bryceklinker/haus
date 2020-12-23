import {Component, OnDestroy, OnInit} from "@angular/core";
import {DiagnosticsMessageModel, DiagnosticsService} from "../../../shared/diagnostics";
import {Observable} from "rxjs";

@Component({
  selector: 'diagnostics-root',
  templateUrl: './diagnostics-root.component.html',
  styleUrls: ['./diagnostics-root.component.scss']
})
export class DiagnosticsRootComponent implements OnInit, OnDestroy {
  get messages$(): Observable<DiagnosticsMessageModel[]> {
    return this.service.messages$;
  }

  get isConnected$(): Observable<boolean> {
    return this.service.isConnected$;
  }

  get allowDiscovery$(): Observable<boolean> {
    return this.service.allowDiscovery$;
  }

  constructor(private readonly service: DiagnosticsService) {
  }

  ngOnInit(): void {
    this.service.start();
  }

  ngOnDestroy(): void {
    this.service.stop();
  }

  onStartDiscovery() {
    this.service.startDiscovery();
  }

  onStopDiscovery() {
    this.service.stopDiscovery();
  }

  onSyncDiscovery() {
    this.service.syncDiscovery();
  }
}
