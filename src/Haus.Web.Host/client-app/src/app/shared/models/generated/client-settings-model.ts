import {AuthSettingsModel} from './auth-settings-model';

export interface ClientSettingsModel {
	version: string;
	auth: AuthSettingsModel;
}
