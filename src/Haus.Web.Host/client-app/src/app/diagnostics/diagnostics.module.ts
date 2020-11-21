import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {DIAGNOSTICS_COMPONENTS} from "./components";
import {EffectsModule} from "@ngrx/effects";
import {DiagnosticsEffects} from "./effects/diagnostics.effects";
import {StoreModule} from "@ngrx/store";
import {DIAGNOSTICS_FEATURE_KEY, diagnosticsReducer} from "./reducers/diagnostics.reducer";
import {SharedModule} from "../shared/shared.module";
import {DiagnosticsRoutingModule} from "./diagnostics-routing.module";

@NgModule({
  declarations: [
    ...DIAGNOSTICS_COMPONENTS
  ],
  imports: [
    CommonModule,
    SharedModule,
    DiagnosticsRoutingModule,
    StoreModule.forFeature(DIAGNOSTICS_FEATURE_KEY, diagnosticsReducer),
    EffectsModule.forFeature([DiagnosticsEffects])
  ]
})
export class DiagnosticsModule { }
