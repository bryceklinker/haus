import {Injectable} from "@angular/core";
import {SettingsModel} from "./settings.model";
import {HttpBackend, HttpClient} from "@angular/common/http";
import {AuthClientConfig} from "@auth0/auth0-angular";
import {BehaviorSubject, Observable} from "rxjs";
import {AuthConfig} from "@auth0/auth0-angular/lib/auth.config";
import {filter, map} from "rxjs/operators";

@Injectable()
export class SettingsService {
  private static _settingsSubject = new BehaviorSubject<SettingsModel | null>(null);

  get settings$(): Observable<SettingsModel> {
    return SettingsService._settingsSubject.asObservable().pipe(
      filter(settings => settings !== null),
      map<SettingsModel | null, SettingsModel>(s => s as SettingsModel)
    )
  }

  get settings(): SettingsModel {
    const value = SettingsService._settingsSubject.getValue();
    if (value) {
      return value;
    }

    throw new Error('Settings are not available yet');
  }

  static init(httpBackend: HttpBackend, config: AuthClientConfig) {
    return () => {
      const http = new HttpClient(httpBackend);
      return http.get<SettingsModel>('/settings').toPromise()
        .then(settings => {
          SettingsService._settingsSubject.next(settings);
          config.set(SettingsService.createAuthConfig(settings));
        })
    }
  }

  private static createAuthConfig(settings: SettingsModel): AuthConfig {
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
