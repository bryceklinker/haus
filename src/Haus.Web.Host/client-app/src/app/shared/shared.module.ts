import {NgModule} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";

import {SettingsService} from "./settings";
import {ThemeService} from "./theming/theme.service";
import {HausApiClient} from "./rest-api";
import {SHARED_COMPONENTS} from "./components";
import {SignalrHubConnectionFactory, SignalrServiceFactory} from "./signalr";
import {MaterialModule} from "./material.module";

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
    ReactiveFormsModule
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
