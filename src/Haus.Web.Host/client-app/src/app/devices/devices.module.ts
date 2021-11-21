import {NgModule} from "@angular/core";
import {DEVICES_COMPONENTS} from "./components";
import {DevicesRoutingModule} from "./devices-routing.module";
import {CommonModule} from "@angular/common";
import {SharedModule} from "../shared/shared.module";

@NgModule({
  declarations: [
    ...DEVICES_COMPONENTS
  ],
  imports: [
    CommonModule,
    SharedModule,
    DevicesRoutingModule
  ],
  exports: [
    SharedModule
  ]
})
export class DevicesModule {

}
