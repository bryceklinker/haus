import {EntityState} from "@ngrx/entity";
import {DiagnosticsMessageModel} from "../../shared/diagnostics";

export interface DiagnosticsState extends EntityState<DiagnosticsMessageModel> {
  isConnected: boolean;
}
