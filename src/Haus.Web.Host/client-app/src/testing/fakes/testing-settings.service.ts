import {SettingsModel, SettingsService} from "../../app/shared/settings";
import {Injectable} from "@angular/core";

@Injectable()
export class TestingSettingsService extends SettingsService {
  private static _settings: SettingsModel = {
    auth: {
      clientId: 'some-client',
      domain: 'some-domain.com'
    },
    deviceSimulator: {
      isEnabled: true,
      url: 'https://something.com'
    }
  };

  get settings(): SettingsModel {
    return TestingSettingsService._settings;
  }

  updateSettings(settings: Partial<SettingsModel>) {
    TestingSettingsService.updateSettings(settings);
  }

  static updateSettings(settings: Partial<SettingsModel>) {
    TestingSettingsService._settings = {
      ...this._settings,
      ...settings
    }
  }
}
