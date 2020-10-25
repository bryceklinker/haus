import {createAction} from '../actions';

export const MQTT_ACTIONS = {
    messageReceived: createAction('[MQTT] Message Received'),
    startingConnection: createAction('[MQTT] Starting Connection'),
    connected: createAction('[MQTT] Connected')
}
