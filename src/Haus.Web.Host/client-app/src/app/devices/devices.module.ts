import {NgModule} from "@angular/core";
import {CommonModule} from "@angular/common";
import {SharedModule} from "../shared/shared.module";
import {AuthModule} from "@auth0/auth0-angular";
import {StoreModule} from "@ngrx/store";
import {EffectsModule} from "@ngrx/effects";
import {DEVICES_FEATURE_KEY, devicesReducer} from "./reducers/devices.reducer";
import {DEVICES_COMPONENTS} from "./components";
import {DEVICES_EFFECTS} from "./effects";
import {DevicesRoutingModule} from "./devices-routing.module";

@NgModule({
  declarations: [
    ...DEVICES_COMPONENTS
  ],
  imports: [
    CommonModule,
    SharedModule,
    AuthModule,
    DevicesRoutingModule,
    StoreModule.forFeature(DEVICES_FEATURE_KEY, devicesReducer),
    EffectsModule.forFeature(DEVICES_EFFECTS),
  ]
})
export class DevicesModule {

}
