import {NgModule} from "@angular/core";
import {StoreModule} from "@ngrx/store";
import {DEVICES_FEATURE_KEY, devicesReducer} from "./reducers/devices.reducer";
import {EffectsModule} from "@ngrx/effects";
import {DEVICES_EFFECTS} from "./effects";
import {CommonModule} from "@angular/common";
import {SharedModule} from "../shared/shared.module";
import {AuthModule} from "@auth0/auth0-angular";

@NgModule({
  imports: [
    AuthModule,
    CommonModule,
    SharedModule,
    StoreModule.forFeature(DEVICES_FEATURE_KEY, devicesReducer),
    EffectsModule.forFeature(DEVICES_EFFECTS),
  ],
  exports: [
    CommonModule,
    SharedModule,
    AuthModule
  ]
})
export class DevicesCoreModule {

}