import {NgModule} from "@angular/core";
import {CommonModule} from "@angular/common";
import {SharedModule} from "../shared/shared.module";
import {HEALTH_COMPONENTS} from "./components";
import {HealthRoutingModule} from "./health-routing.module";

@NgModule({
  imports: [
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
