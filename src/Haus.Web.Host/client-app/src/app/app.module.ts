import {NgModule} from '@angular/core';
import {StoreModule} from '@ngrx/store';
import {EffectsModule} from '@ngrx/effects';
import {StoreDevtoolsModule} from '@ngrx/store-devtools';
import {routerReducer, StoreRouterConnectingModule} from '@ngrx/router-store';
import {AuthHttpInterceptor, AuthModule} from "@auth0/auth0-angular";
import {SignalREffects, signalrReducer} from "ngrx-signalr-core";

import {AppRoutingModule} from './app-routing.module';
import {environment} from '../environments/environment';
import {SharedModule} from "./shared/shared.module";
import {SHELL_COMPONENTS} from "./shell/components";
import {DiagnosticsModule} from "./diagnostics/diagnostics.module";
import {CommonModule} from "@angular/common";
import {HTTP_INTERCEPTORS} from "@angular/common/http";
import {SimpleRouterSerializer} from "./shared/routing";
import {AppState} from "./app.state";

@NgModule({
  declarations: [
    ...SHELL_COMPONENTS
  ],
  imports: [
    CommonModule,
    AppRoutingModule,
    SharedModule,
    StoreModule.forRoot<Partial<AppState>>({
      signalr: signalrReducer,
      router: routerReducer
    }),
    StoreDevtoolsModule.instrument({maxAge: 25, logOnly: environment.production}),
    EffectsModule.forRoot([SignalREffects]),
    StoreRouterConnectingModule.forRoot({
      serializer: SimpleRouterSerializer
    }),
    AuthModule,
    DiagnosticsModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthHttpInterceptor,
      multi: true,
    }
  ]
})
export class AppModule {
}
