import {APP_INITIALIZER, NgModule} from '@angular/core';
import {AuthClientConfig, AuthHttpInterceptor, AuthModule} from "@auth0/auth0-angular";
import {CommonModule} from "@angular/common";
import {BrowserModule} from "@angular/platform-browser";
import {HTTP_INTERCEPTORS, HttpBackend, HttpClientModule} from "@angular/common/http";
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";

import {AppRoutingModule} from './app-routing.module';
import {SharedModule} from "./shared/shared.module";
import {SHELL_COMPONENTS} from "./shell/components";
import {SettingsService} from "./shared/settings";
import {ShellComponent} from "./shell/components/shell/shell.component";
import {SHELL_PROVIDERS} from "./shell/services";
import {StoreModule} from "@ngrx/store";
import {AppState} from "./app.state";
import {EffectsModule} from "@ngrx/effects";
import {appReducerMap} from "./app-reducer-map";
import {APP_EFFECTS} from "./app-effects";

@NgModule({
  declarations: [
    ...SHELL_COMPONENTS
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    CommonModule,
    AppRoutingModule,
    AuthModule.forRoot(),
    HttpClientModule,
    SharedModule,
    StoreModule.forRoot<AppState>(appReducerMap),
    EffectsModule.forRoot(APP_EFFECTS)
  ],
  bootstrap: [ShellComponent],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: SettingsService.init,
      deps: [HttpBackend, AuthClientConfig],
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthHttpInterceptor,
      multi: true,
    },
    ...SHELL_PROVIDERS
  ]
})
export class AppModule {
}
