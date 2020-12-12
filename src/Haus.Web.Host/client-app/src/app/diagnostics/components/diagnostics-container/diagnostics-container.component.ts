import {Component, OnInit, OnDestroy} from "@angular/core";
import {AppState} from "../../../app.state";
import {Store} from "@ngrx/store";
import {Observable} from "rxjs";
import {
  selectDiagnosticsAllowDiscovery,
  selectDiagnosticsMessages,
  selectIsDiagnosticHubConnected
} from "../../reducers/diagnostics.reducer";
import {DiagnosticsActions} from "../../actions";
import {DiagnosticsMessageModel} from "../../models";

@Component({
  selector: 'diagnostics-container',
  templateUrl: './diagnostics-container.component.html',
  styleUrls: ['./diagnostics-container.component.scss']
})
export class DiagnosticsContainerComponent implements OnInit, OnDestroy {
  isDiagnosticsConnected$: Observable<boolean>;
  diagnosticMessages$: Observable<Array<DiagnosticsMessageModel>>;
  allowDiscovery$: Observable<boolean>;

  constructor(private store: Store<AppState>) {
    this.isDiagnosticsConnected$ = store.select(selectIsDiagnosticHubConnected);
    this.diagnosticMessages$ = store.select(selectDiagnosticsMessages);
    this.allowDiscovery$ = store.select(selectDiagnosticsAllowDiscovery);
  }

  replayMessage($event: DiagnosticsMessageModel) {
    this.store.dispatch(DiagnosticsActions.replay.request($event));
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

  ngOnInit(): void {
    this.store.dispatch(DiagnosticsActions.initHub());
  }

  ngOnDestroy(): void {
    this.store.dispatch(DiagnosticsActions.disconnectHub());
  }
}
