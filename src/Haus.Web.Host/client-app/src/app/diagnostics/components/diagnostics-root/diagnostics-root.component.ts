import {Component, OnDestroy, OnInit} from "@angular/core";
import {Observable} from "rxjs";
import {AppState} from "../../../app.state";
import {Store} from "@ngrx/store";
import {
  DiagnosticsActions,
  selectAllDiagnosticMessages,
  selectAllowDiagnosticsDiscovery,
  selectIsDiagnosticsConnected
} from "../../state";
import {MqttDiagnosticsMessageModel, UiMqttDiagnosticsMessageModel} from "../../../shared/models";

@Component({
  selector: 'diagnostics-root',
  templateUrl: './diagnostics-root.component.html',
  styleUrls: ['./diagnostics-root.component.scss']
})
export class DiagnosticsRootComponent implements OnInit, OnDestroy {
  messages$: Observable<UiMqttDiagnosticsMessageModel[]>;
  isConnected$: Observable<boolean>;
  allowDiscovery$: Observable<boolean>;

  constructor(private readonly store: Store<AppState>) {
    this.messages$ = this.store.select(selectAllDiagnosticMessages);
    this.isConnected$ = this.store.select(selectIsDiagnosticsConnected);
    this.allowDiscovery$ = this.store.select(selectAllowDiagnosticsDiscovery);
  }

  ngOnInit(): void {
    this.store.dispatch(DiagnosticsActions.start());
  }

  ngOnDestroy(): void {
    this.store.dispatch(DiagnosticsActions.stop());
  }

  onStartDiscovery() {
    this.store.dispatch(DiagnosticsActions.startDiscovery.request());
  }

  onStopDiscovery() {
    this.store.dispatch(DiagnosticsActions.stopDiscovery.request());
  }

  onSyncDiscovery() {
    this.store.dispatch(DiagnosticsActions.syncDiscovery.request());
  }

  onReplayMessage($event: UiMqttDiagnosticsMessageModel) {
    this.store.dispatch(DiagnosticsActions.replayMessage.request($event));
  }
}
