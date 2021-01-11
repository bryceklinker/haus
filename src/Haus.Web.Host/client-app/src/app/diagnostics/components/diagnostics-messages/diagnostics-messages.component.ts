import {Component, EventEmitter, Input, Output} from "@angular/core";
import {UiMqttDiagnosticsMessageModel} from "../../../shared/models";

@Component({
  selector: 'diagnostics-messages',
  templateUrl: './diagnostics-messages.component.html',
  styleUrls: ['./diagnostics-messages.component.scss']
})
export class DiagnosticsMessagesComponent {
  @Input() messages: Array<UiMqttDiagnosticsMessageModel> | null = [];
  @Output() replayMessage = new EventEmitter<UiMqttDiagnosticsMessageModel>();

  onReplay(message: UiMqttDiagnosticsMessageModel) {
    this.replayMessage.emit(message);
  }
}
