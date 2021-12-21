import {Component, EventEmitter, Input, Output} from "@angular/core";
import {UiMqttDiagnosticsMessageModel} from "../../../shared/models";
import { DiagnosticsFilterParams } from "../../models";

@Component({
  selector: 'diagnostics-messages',
  templateUrl: './diagnostics-messages.component.html',
  styleUrls: ['./diagnostics-messages.component.scss']
})
export class DiagnosticsMessagesComponent {
  @Input() messages: Array<UiMqttDiagnosticsMessageModel> | null = [];
  @Input() filterParams: DiagnosticsFilterParams | null = null;
  @Output() replayMessage = new EventEmitter<UiMqttDiagnosticsMessageModel>();

  get filteredMessages(): Array<UiMqttDiagnosticsMessageModel> {
    if  (!this.messages) {
      return [];
    }

    return this.messages.filter(message => this.shouldShowMessage(message));
  }

  onReplay(message: UiMqttDiagnosticsMessageModel) {
    this.replayMessage.emit(message);
  }

  private shouldShowMessage(message: UiMqttDiagnosticsMessageModel): boolean {
    if  (!this.filterParams) {
      return true;
    }

    return message.topic.includes(this.filterParams.topic || '');
  }
}
