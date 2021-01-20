import {SettingsService} from "../../app/shared/settings";
import {Injectable} from "@angular/core";
import {ClientSettingsModel} from "../../app/shared/models";

const DEFAULT_TESTING_SETTINGS: ClientSettingsModel = {
  version: '',
  auth: {
    clientId: 'some-client',
    domain: 'some-domain.com',
    audience: ''
  },
};

@Injectable()
export class TestingSettingsService extends SettingsService {
  constructor() {
    super();
    this.updateSettings(DEFAULT_TESTING_SETTINGS);
  }
}
