import {Component, Input} from "@angular/core";
import {MqttDiagnosticsMessageModel} from "../../models/mqtt-diagnostics-message.model";

@Component({
  selector: 'diagnostics-messages',
  templateUrl: './diagnostics-messages.component.html',
  styleUrls: ['./diagnostics-messages.component.scss']
})
export class DiagnosticsMessagesComponent {
  @Input() messages: Array<MqttDiagnosticsMessageModel> = [];
}
