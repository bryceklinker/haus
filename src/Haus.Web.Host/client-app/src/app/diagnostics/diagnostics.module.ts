import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {DIAGNOSTICS_COMPONENTS} from "./components";
import {SharedModule} from "../shared/shared.module";
import {DiagnosticsRoutingModule} from "./diagnostics-routing.module";

@NgModule({
  declarations: [
    ...DIAGNOSTICS_COMPONENTS
  ],
  imports: [
    CommonModule,
    SharedModule,
    DiagnosticsRoutingModule
  ]
})
export class DiagnosticsModule { }
