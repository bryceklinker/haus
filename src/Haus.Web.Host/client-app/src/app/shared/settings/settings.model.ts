export interface SettingsModel {
  auth: {
    domain: string;
    clientId: string;
  },
  deviceSimulator: {
    url: string;
    isEnabled: boolean;
  }
}
