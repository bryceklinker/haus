import {NgModule} from "@angular/core";
import {RouterModule, Routes} from "@angular/router";
import {DeviceSimulatorRootComponent} from "./components/device-simulator-root/device-simulator-root.component";
import {AddSimulatedDeviceComponent} from "./components/add-simulated-device/add-simulated-device.component";

const routes: Routes = [
  {
    path: 'add-simulated-device',
    component: AddSimulatedDeviceComponent
  },
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
