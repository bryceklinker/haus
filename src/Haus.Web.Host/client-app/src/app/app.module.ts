import {NgModule} from '@angular/core';
import {AuthHttpInterceptor, AuthModule} from "@auth0/auth0-angular";
import {CommonModule} from "@angular/common";
import {HTTP_INTERCEPTORS} from "@angular/common/http";

import {AppRoutingModule} from './app-routing.module';
import {SharedModule} from "./shared/shared.module";
import {SHELL_COMPONENTS} from "./shell/components";
import {HausDataServiceFactory, HausHttpUrlGeneratorService} from "./shared/services";

@NgModule({
  declarations: [
    ...SHELL_COMPONENTS
  ],
  imports: [
    CommonModule,
    AppRoutingModule,
    AuthModule,
    SharedModule
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
