import {Injectable} from "@angular/core";
import {SettingsModel} from "./settings.model";

@Injectable()
export class SettingsService {
  private _settings: SettingsModel | null;

  get settings(): SettingsModel {
    if (this._settings) {
      return this._settings;
    }

    throw new Error('Settings was not initialized');
  }

  constructor() {
    this._settings = null;
  }

  init(settings: SettingsModel) {
    this._settings = settings;
  }
}
