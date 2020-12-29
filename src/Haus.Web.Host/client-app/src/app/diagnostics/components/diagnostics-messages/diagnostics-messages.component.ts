import {Component, EventEmitter, Input, Output} from "@angular/core";
import {DiagnosticsMessageModel} from "../../models";


@Component({
  selector: 'diagnostics-messages',
  templateUrl: './diagnostics-messages.component.html',
  styleUrls: ['./diagnostics-messages.component.scss']
})
export class DiagnosticsMessagesComponent {
  @Input() messages: Array<DiagnosticsMessageModel> | null = [];
  @Output() replayMessage = new EventEmitter<DiagnosticsMessageModel>();

  onReplay(message: DiagnosticsMessageModel) {
    this.replayMessage.emit(message);
  }
}
