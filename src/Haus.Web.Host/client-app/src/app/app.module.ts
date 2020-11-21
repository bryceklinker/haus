import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {StoreModule} from '@ngrx/store';
import {EffectsModule} from '@ngrx/effects';
import {StoreDevtoolsModule} from '@ngrx/store-devtools';
import {StoreRouterConnectingModule} from '@ngrx/router-store';
import {AuthModule} from "@auth0/auth0-angular";
import {SignalREffects, signalrReducer} from "ngrx-signalr-core";

import {AppRoutingModule} from './app-routing.module';
import {environment} from '../environments/environment';
import {SharedModule} from "./shared/shared.module";
import {SettingsService} from "./shared/settings";
import {SHELL_COMPONENTS} from "./shell/components";
import {ShellComponent} from "./shell/components/shell/shell.component";
import {DiagnosticsModule} from "./diagnostics/diagnostics.module";

@NgModule({
  declarations: [
    ...SHELL_COMPONENTS
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    SharedModule,
    StoreModule.forRoot({ signalr: signalrReducer }),
    StoreDevtoolsModule.instrument({maxAge: 25, logOnly: environment.production}),
    EffectsModule.forRoot([SignalREffects]),
    StoreRouterConnectingModule.forRoot(),
    AuthModule.forRoot({...(SettingsService.getSettings() || {auth: {domain: '', clientId: ''}}).auth }),
    DiagnosticsModule
  ],
  bootstrap: [ShellComponent]
})
export class AppModule {
}
