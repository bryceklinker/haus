import {enableProdMode, NgModule} from '@angular/core';
import {platformBrowserDynamic} from '@angular/platform-browser-dynamic';

import {AppModule} from './app/app.module';
import {environment} from './environments/environment';
import {SettingsService} from "./app/shared/settings";
import {AuthModule} from "@auth0/auth0-angular";
import {ShellComponent} from "./app/shell/components/shell/shell.component";
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";
import {BrowserModule} from "@angular/platform-browser";

if (environment.production) {
  enableProdMode();
}

SettingsService.loadSettings()
  .then(settings => {
    @NgModule({
      imports: [
        BrowserModule,
        AppModule,
        BrowserAnimationsModule,
        AuthModule.forRoot({
          ...settings.auth,
          redirectUri: window.location.origin,
          httpInterceptor: {
            allowedList: [
              '/api/*'
            ]
          }
        })
      ],
      bootstrap: [ShellComponent]
    })
    class Module {

    }
    return Module;
  })
  .then(module => platformBrowserDynamic().bootstrapModule(module))
  .catch(err => console.error('Failed to bootstrap', err));



