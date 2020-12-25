import {NgModule} from "@angular/core";
import {RouterModule, Routes} from "@angular/router";
import {DeviceSimulatorRootComponent, AddSimulatedDeviceComponent} from "./components";

const routes: Routes = [

  {
    path: 'add-simulated-device',
    component: AddSimulatedDeviceComponent
  },
  {
    path: '',
    component: DeviceSimulatorRootComponent,
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
