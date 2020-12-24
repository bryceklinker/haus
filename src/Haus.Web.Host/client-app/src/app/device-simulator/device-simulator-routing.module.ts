import {NgModule} from "@angular/core";
import {RouterModule, Routes} from "@angular/router";
import {DeviceSimulatorRootComponent} from "./components";

const routes: Routes = [
  {
    path: '',
    component: DeviceSimulatorRootComponent
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class DeviceSimulatorRoutingModule {

}
