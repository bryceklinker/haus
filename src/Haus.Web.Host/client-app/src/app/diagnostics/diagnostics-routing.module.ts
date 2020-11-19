import {RouterModule, Routes} from "@angular/router";
import {NgModule} from "@angular/core";
import {DiagnosticsContainerComponent} from "./components/diagnostics-container";

const routes: Routes = [
  {
    path: '',
    component: DiagnosticsContainerComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DiagnosticsRoutingModule {
}
