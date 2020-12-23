import {Component, OnDestroy, OnInit} from "@angular/core";
import {DiagnosticsMessageModel, DiagnosticsService} from "../../../shared/diagnostics";
import {Observable} from "rxjs";
import {DestroyableSubject} from "../../../shared/destroyable-subject";

@Component({
  selector: 'diagnostics-root',
  templateUrl: './diagnostics-root.component.html',
  styleUrls: ['./diagnostics-root.component.scss']
})
export class DiagnosticsRootComponent implements OnInit, OnDestroy {
  private readonly destroyable = new DestroyableSubject();

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
    this.destroyable.register(this.service.start()).subscribe();
  }

  ngOnDestroy(): void {
    this.destroyable.register(this.service.stop()).subscribe();
    this.destroyable.destroy();
  }

  onStartDiscovery() {
    this.destroyable.register(this.service.startDiscovery()).subscribe();
  }

  onStopDiscovery() {
    this.destroyable.register(this.service.stopDiscovery()).subscribe();
  }

  onSyncDiscovery() {
    this.destroyable.register(this.service.syncDiscovery()).subscribe();
  }

  onReplayMessage($event: DiagnosticsMessageModel) {
    this.destroyable.register(this.service.replayMessage($event)).subscribe();
  }
}
