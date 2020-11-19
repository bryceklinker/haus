import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {DIAGNOSTICS_COMPONENTS} from "./components";
import {EffectsModule} from "@ngrx/effects";
import {DiagnosticsEffects} from "./effects/diagnostics.effects";

@NgModule({
  declarations: [
    ...DIAGNOSTICS_COMPONENTS
  ],
  imports: [
    CommonModule,
    EffectsModule.forFeature([DiagnosticsEffects])
  ]
})
export class DiagnosticsModule { }
