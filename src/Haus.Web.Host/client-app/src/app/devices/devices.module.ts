import {NgModule} from "@angular/core";
import {DEVICES_COMPONENTS} from "./components";
import {DevicesRoutingModule} from "./devices-routing.module";
import {AuthModule} from "@auth0/auth0-angular";
import {CommonModule} from "@angular/common";
import {SharedModule} from "../shared/shared.module";

@NgModule({
  declarations: [
    ...DEVICES_COMPONENTS
  ],
  imports: [
    AuthModule,
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
