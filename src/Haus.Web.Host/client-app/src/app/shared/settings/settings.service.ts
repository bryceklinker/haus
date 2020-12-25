import {Injectable} from "@angular/core";
import {SettingsModel} from "./settings.model";
import {HttpBackend, HttpClient} from "@angular/common/http";
import {AuthClientConfig} from "@auth0/auth0-angular";
import {fromObservableToPromise} from "../observable-extensions";

let settingsModel: SettingsModel | null = null;

@Injectable()
export class SettingsService {
  static getSettings() {
    return settingsModel
  }

  get settings(): SettingsModel {
    if (!settingsModel) {
      throw new Error('Settings have not been set the app will not work correctly with out settings')
    }
    return settingsModel;
  }

  static async loadSettings(): Promise<SettingsModel> {
    const response = await fetch('/settings');
    return settingsModel = await response.json();
  }
}

export function settingsInitializer(httpBackend: HttpBackend, config: AuthClientConfig) {
  return () => {
    const http = new HttpClient(httpBackend);
    return fromObservableToPromise(http.get<SettingsModel>('/settings'))
      .then(settings => {
        settingsModel = settings;
        config.set({
          ...settings.auth,
          redirectUri: window.location.origin,
          httpInterceptor: {
            allowedList: [
              '/api/*',
              'api/*'
            ]
          }
        });
      })
  }
}
