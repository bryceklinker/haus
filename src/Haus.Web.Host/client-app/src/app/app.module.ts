import {APP_INITIALIZER, NgModule} from '@angular/core';
import {AuthClientConfig, AuthHttpInterceptor, AuthModule} from "@auth0/auth0-angular";
import {CommonModule} from "@angular/common";

import {AppRoutingModule} from './app-routing.module';
import {SharedModule} from "./shared/shared.module";
import {SHELL_COMPONENTS} from "./shell/components";
import {HTTP_INTERCEPTORS, HttpBackend, HttpClientModule} from "@angular/common/http";
import {SettingsService} from "./shared/settings";
import {BrowserModule} from "@angular/platform-browser";
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";
import {ShellComponent} from "./shell/components/shell/shell.component";
import {SHELL_PROVIDERS} from "./shell/services";

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
    SharedModule
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
