import {NgModule} from "@angular/core";
import {AuthModule} from "@auth0/auth0-angular";
import {CommonModule} from "@angular/common";
import {SharedModule} from "../shared/shared.module";
import {DeviceSimulatorRoutingModule} from "./device-simulator-routing.module";
import {DEVICE_SIMULATOR_PROVIDERS} from "./services";
import {DEVICE_SIMULATOR_COMPONENTS} from "./components";

@NgModule({
  imports: [
    AuthModule,
    CommonModule,
    SharedModule,
    DeviceSimulatorRoutingModule
  ],
  declarations: [
    ...DEVICE_SIMULATOR_COMPONENTS
  ],
  providers: [
    ...DEVICE_SIMULATOR_PROVIDERS
  ]
})
export class DeviceSimulatorModule {

}
