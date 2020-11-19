import {HttpClient} from "@angular/common/http";
import {AuthClientConfig} from "@auth0/auth0-angular";
import {SettingsModel} from "./settings.model";
import {SettingsService} from "./settings.service";

export function settingsInitializer(http: HttpClient, config: AuthClientConfig, settingsService: SettingsService) {
  return async () => {
    const response = await http.get<SettingsModel>('/settings').toPromise();
    config.set({
      ...response.auth,
      httpInterceptor: {
        allowedList: [
          '/*'
        ]
      }
    });
    settingsService.init(response);
  }
}

settingsInitializer.DEPS = [HttpClient, AuthClientConfig, SettingsService];
