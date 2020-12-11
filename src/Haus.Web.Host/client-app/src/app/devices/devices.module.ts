import {NgModule} from "@angular/core";
import {DEVICES_COMPONENTS} from "./components";
import {DevicesRoutingModule} from "./devices-routing.module";
import {DevicesCoreModule} from "./devices-core.module";

@NgModule({
  declarations: [
    ...DEVICES_COMPONENTS
  ],
  imports: [
    DevicesCoreModule,
    DevicesRoutingModule,
  ]
})
export class DevicesModule {

}
