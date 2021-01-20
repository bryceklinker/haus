import {ExpandoObject} from './expando-object';

export interface LogEntryModel {
	timestamp: string;
	level: string;
	message: string;
	value: ExpandoObject;
}
