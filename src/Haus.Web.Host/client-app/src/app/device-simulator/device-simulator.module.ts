import {NgModule} from "@angular/core";
import {CommonModule} from "@angular/common";
import {SharedModule} from "../shared/shared.module";
import {DeviceSimulatorRoutingModule} from "./device-simulator-routing.module";
import {DEVICE_SIMULATOR_COMPONENTS} from "./components";

@NgModule({
  declarations: [
    ...DEVICE_SIMULATOR_COMPONENTS
  ],
  imports: [
    CommonModule,
    SharedModule,
    DeviceSimulatorRoutingModule
  ],
  exports: [
    SharedModule
  ]
})
export class DeviceSimulatorModule {

}
