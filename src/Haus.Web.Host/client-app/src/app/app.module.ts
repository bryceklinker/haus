import {BrowserModule} from '@angular/platform-browser';
import {NgModule, APP_INITIALIZER} from '@angular/core';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {StoreModule} from '@ngrx/store';
import {EffectsModule} from '@ngrx/effects';
import {StoreDevtoolsModule} from '@ngrx/store-devtools';
import {StoreRouterConnectingModule} from '@ngrx/router-store';

import {AppRoutingModule} from './app-routing.module';
import {environment} from '../environments/environment';
import {SharedModule} from "./shared/shared.module";
import {settingsInitializer} from "./shared/settings";
import {AuthModule} from "@auth0/auth0-angular";
import {SHELL_COMPONENTS} from "./shell/components";
import {ShellComponent} from "./shell/components/shell/shell.component";
import {SignalREffects, signalrReducer} from "ngrx-signalr-core";
import {AppState} from "./app.state";

@NgModule({
  declarations: [
    ...SHELL_COMPONENTS
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    SharedModule,
    StoreModule.forRoot<AppState>({
      signalr: signalrReducer
    }),
    StoreDevtoolsModule.instrument({maxAge: 25, logOnly: environment.production}),
    EffectsModule.forRoot([SignalREffects]),
    StoreRouterConnectingModule.forRoot(),
    AuthModule.forRoot(),
  ],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: settingsInitializer,
      deps: settingsInitializer.DEPS,
      multi: true
    }
  ],
  bootstrap: [ShellComponent]
})
export class AppModule {
}
