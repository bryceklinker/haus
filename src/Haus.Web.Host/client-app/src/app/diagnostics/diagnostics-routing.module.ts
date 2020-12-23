import {RouterModule, Routes} from "@angular/router";
import {NgModule} from "@angular/core";
import {DiagnosticsRootComponent} from "./components/diagnostics-root/diagnostics-root.component";

const routes: Routes = [
  {
    path: '',
    component: DiagnosticsRootComponent,
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DiagnosticsRoutingModule {
}
