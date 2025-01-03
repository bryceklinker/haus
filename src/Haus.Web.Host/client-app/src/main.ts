import {enableProdMode} from '@angular/core';
import {platformBrowserDynamic} from '@angular/platform-browser-dynamic';

import {AppModule} from './app/app.module';
import {environment} from './environments/environment';

async function main() {
  if (environment.production) {
    enableProdMode();
  }

  await platformBrowserDynamic().bootstrapModule(AppModule);
}

main()
  .catch(err => console.error(err));



