import {Component} from "@angular/core";
import {AppState} from "../../../app.state";
import {Store} from "@ngrx/store";
import {Observable} from "rxjs";
import {selectDiagnosticsMessages, selectIsDiagnosticHubConnected} from "../../reducers/diagnostics.reducer";
import {MqttDiagnosticsMessageModel} from "../../models/mqtt-diagnostics-message.model";

@Component({
  selector: 'diagnostics-container',
  templateUrl: './diagnostics-container.component.html',
  styleUrls: ['./diagnostics-container.component.scss']
})
export class DiagnosticsContainerComponent {
  isDiagnosticsConnected$: Observable<boolean>;
  diagnosticMessages$: Observable<Array<MqttDiagnosticsMessageModel>>;

  constructor(private store: Store<AppState>) {
    this.isDiagnosticsConnected$ = store.select(selectIsDiagnosticHubConnected);
    this.diagnosticMessages$ = store.select(selectDiagnosticsMessages);
  }
}
