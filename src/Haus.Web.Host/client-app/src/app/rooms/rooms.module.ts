import {NgModule} from "@angular/core";
import {CommonModule} from "@angular/common";

import {SharedModule} from "../shared/shared.module";
import {RoomsRoutingModule} from "./rooms-routing.module";
import {ROOMS_COMPONENTS} from "./components";

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    RoomsRoutingModule
  ],
  declarations: [
    ...ROOMS_COMPONENTS
  ]
})
export class RoomsModule {
}
