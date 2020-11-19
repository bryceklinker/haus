import {enableProdMode} from '@angular/core';
import {platformBrowserDynamic} from '@angular/platform-browser-dynamic';

import {AppModule} from './app/app.module';
import {environment} from './environments/environment';
import {SettingsService} from "./app/shared/settings";

async function bootstrap() {
  if (environment.production) {
    enableProdMode();
  }
  await SettingsService.loadSettings();

  await platformBrowserDynamic()
    .bootstrapModule(AppModule);
}

bootstrap()
  .catch(err => console.error('Failed to bootstrap application', err));



