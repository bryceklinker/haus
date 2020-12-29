import {NgModule} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";

import {SettingsService} from "./settings";
import {ThemeService} from "./theming/theme.service";
import {HausApiClient} from "./rest-api";
import {SHARED_COMPONENTS} from "./components";
import {SignalrHubConnectionFactory, SignalrServiceFactory} from "./signalr";
import {MaterialModule} from "./material.module";
import {StoreModule} from "@ngrx/store";
import {AppState} from "../app.state";
import {EffectsModule} from "@ngrx/effects";
import {DevicesEffects} from "../devices/effects/devices.effects";
import {sharedReducerMap} from "./shared-reducer-map";
import {DiagnosticsEffects} from "../diagnostics/effects/diagnostics.effects";
import {RoomsEffects} from "../rooms/effects/rooms.effects";

@NgModule({
  providers: [
    SettingsService,
    ThemeService,
    HausApiClient,
    SignalrServiceFactory,
    SignalrHubConnectionFactory
  ],
  imports: [
    MaterialModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    StoreModule.forRoot<AppState>(sharedReducerMap),
    EffectsModule.forRoot([
      DevicesEffects,
      DiagnosticsEffects,
      RoomsEffects
    ])
  ],
  exports: [
    MaterialModule,
    FormsModule,
    ReactiveFormsModule,
    ...SHARED_COMPONENTS
  ],
  declarations: [
    ...SHARED_COMPONENTS
  ]
})
export class SharedModule {
}
