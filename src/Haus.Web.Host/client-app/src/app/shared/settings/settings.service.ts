import {Injectable} from "@angular/core";
import {HttpBackend, HttpClient} from "@angular/common/http";
import {AuthClientConfig} from "@auth0/auth0-angular";
import {BehaviorSubject, Observable} from "rxjs";
import {AuthConfig} from "@auth0/auth0-angular/lib/auth.config";
import {filter, map} from "rxjs/operators";
import {ClientSettingsModel} from "../models";

@Injectable()
export class SettingsService {
  protected static _settingsSubject = new BehaviorSubject<ClientSettingsModel | null>(null);

  get settings$(): Observable<ClientSettingsModel> {
    return SettingsService._settingsSubject.asObservable().pipe(
      filter(settings => settings !== null),
      map<ClientSettingsModel | null, ClientSettingsModel>(s => s as ClientSettingsModel)
    )
  }

  get settings(): ClientSettingsModel {
    const value = SettingsService._settingsSubject.getValue();
    if (value) {
      return value;
    }

    throw new Error('Settings are not available yet');
  }

  updateSettings(settings: ClientSettingsModel) {
    SettingsService._settingsSubject.next(settings);
  }

  static init(httpBackend: HttpBackend, config: AuthClientConfig) {
    return () => {
      const http = new HttpClient(httpBackend);
      return http.get<ClientSettingsModel>('/client-settings').toPromise()
        .then(settings => {
          SettingsService._settingsSubject.next(settings);
          config.set(SettingsService.createAuthConfig(settings));
        })
    }
  }

  private static createAuthConfig(settings: ClientSettingsModel): AuthConfig {
    return {
      ...settings.auth,
      redirectUri: window.location.origin,
      httpInterceptor: {
        allowedList: [
          '/api/*',
          'api/*'
        ]
      }
    };
  }
}
