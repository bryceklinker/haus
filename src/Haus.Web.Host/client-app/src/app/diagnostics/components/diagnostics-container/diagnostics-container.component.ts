import {Component, OnInit} from "@angular/core";
import {AppState} from "../../../app.state";
import {Store} from "@ngrx/store";
import {Observable} from "rxjs";
import {selectDiagnosticsMessages, selectIsDiagnosticHubConnected} from "../../reducers/diagnostics.reducer";
import {DiagnosticsActions} from "../../actions";
import {DiagnosticsMessageModel} from "../../models";

@Component({
  selector: 'diagnostics-container',
  templateUrl: './diagnostics-container.component.html',
  styleUrls: ['./diagnostics-container.component.scss']
})
export class DiagnosticsContainerComponent implements OnInit {
  isDiagnosticsConnected$: Observable<boolean>;
  diagnosticMessages$: Observable<Array<DiagnosticsMessageModel>>;

  constructor(private store: Store<AppState>) {
    this.isDiagnosticsConnected$ = store.select(selectIsDiagnosticHubConnected);
    this.diagnosticMessages$ = store.select(selectDiagnosticsMessages);
  }

  replayMessage($event: DiagnosticsMessageModel) {
    this.store.dispatch(DiagnosticsActions.replay.request($event));
  }

  ngOnInit(): void {
    this.store.dispatch(DiagnosticsActions.initHub());
  }
}
