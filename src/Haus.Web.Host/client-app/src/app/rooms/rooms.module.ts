import {NgModule} from "@angular/core";
import {AuthModule} from "@auth0/auth0-angular";
import {CommonModule} from "@angular/common";

import {SharedModule} from "../shared/shared.module";
import {RoomsRoutingModule} from "./rooms-routing.module";
import {ROOMS_COMPONENTS} from "./components";
import {EntityDefinitionService} from "@ngrx/data";

@NgModule({
  imports: [
    AuthModule,
    CommonModule,
    SharedModule,
    RoomsRoutingModule
  ],
  declarations: [
    ...ROOMS_COMPONENTS
  ]
})
export class RoomsModule {
  constructor(private eds: EntityDefinitionService) {
    eds.registerMetadata({
      entityName: 'Room'
    })
  }
}
