import {Injectable} from "@angular/core";
import {HttpBackend, HttpClient} from "@angular/common/http";
import {AuthClientConfig} from "@auth0/auth0-angular";
import {BehaviorSubject, Observable} from "rxjs";
import {filter, map} from "rxjs/operators";
import {ClientSettingsModel} from "../models";
import {HausAuthService} from '../auth/services';

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
          if (!settings) {
            return;
          }
          HausAuthService.initialize(settings, config);
          SettingsService._settingsSubject.next(settings || null);
        })
    }
  }
}
