import {NgModule} from "@angular/core";
import {AuthModule} from "@auth0/auth0-angular";
import {CommonModule} from "@angular/common";
import {SharedModule} from "../shared/shared.module";
import {HEALTH_COMPONENTS} from "./components";
import {HealthRoutingModule} from "./health-routing.module";

@NgModule({
  imports: [
    AuthModule,
    CommonModule,
    SharedModule,
    HealthRoutingModule
  ],
  declarations: [
    ...HEALTH_COMPONENTS
  ]
})
export class HealthModule {

}
