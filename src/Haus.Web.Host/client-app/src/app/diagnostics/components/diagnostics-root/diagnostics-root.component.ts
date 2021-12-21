import {Component, OnDestroy, OnInit} from "@angular/core";
import {Observable} from "rxjs";
import {Store} from "@ngrx/store";

import {AppState} from "../../../app.state";
import {
  DiagnosticsActions,
  selectAllDiagnosticMessages,
  selectIsDiagnosticsConnected
} from "../../state";
import {UiMqttDiagnosticsMessageModel} from "../../../shared/models";
import {DiscoveryActions, selectIsDiscoveryAllowed} from "../../../shared/discovery";
import { DiagnosticsFilterParams } from "../../models";

@Component({
  selector: 'diagnostics-root',
  templateUrl: './diagnostics-root.component.html',
  styleUrls: ['./diagnostics-root.component.scss']
})
export class DiagnosticsRootComponent implements OnInit, OnDestroy {
  messages$: Observable<UiMqttDiagnosticsMessageModel[]>;
  filterParams: DiagnosticsFilterParams | null = null;
  isConnected$: Observable<boolean>;
  allowDiscovery$: Observable<boolean>;

  constructor(private readonly store: Store<AppState>) {
    this.messages$ = this.store.select(selectAllDiagnosticMessages);
    this.isConnected$ = this.store.select(selectIsDiagnosticsConnected);
    this.allowDiscovery$ = this.store.select(selectIsDiscoveryAllowed);
  }

  ngOnInit(): void {
    this.store.dispatch(DiagnosticsActions.start());
  }

  ngOnDestroy(): void {
    this.store.dispatch(DiagnosticsActions.stop());
  }

  onStartDiscovery() {
    this.store.dispatch(DiscoveryActions.startDiscovery.request());
  }

  onStopDiscovery() {
    this.store.dispatch(DiscoveryActions.stopDiscovery.request());
  }

  onSyncDiscovery() {
    this.store.dispatch(DiscoveryActions.syncDiscovery.request());
  }

  onReplayMessage($event: UiMqttDiagnosticsMessageModel) {
    this.store.dispatch(DiagnosticsActions.replayMessage.request($event));
  }

  onFilterChange(params: DiagnosticsFilterParams) {
    this.filterParams = params;
  }
}
