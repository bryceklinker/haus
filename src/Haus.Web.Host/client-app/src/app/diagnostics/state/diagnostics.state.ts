import {EntityState} from "@ngrx/entity";
import {DiagnosticsMessageModel} from "../models";

export interface DiagnosticsState extends EntityState<DiagnosticsMessageModel> {
  isConnected: boolean;
}
