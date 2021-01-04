import {EntityState} from "@ngrx/entity";
import {UiMqttDiagnosticsMessageModel} from "../../shared/models";

export interface DiagnosticsState extends EntityState<UiMqttDiagnosticsMessageModel> {
  isConnected: boolean;
}
