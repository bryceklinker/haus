import {Injectable} from "@angular/core";
import {SettingsModel} from "./settings.model";

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
