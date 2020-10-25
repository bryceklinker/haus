import {MQTT_ACTIONS} from './actions';

const CONNECTION_STATUS = {
    NOT_CONNECTED: 'not connected',
    CONNECTING: 'connecting',
    CONNECTED: 'connected'
}

export const initialState = {
    connectionStatus: CONNECTION_STATUS.NOT_CONNECTED,
    messages: []
}

export function mqttReducer(state = initialState, {type, payload} = {}) {
    switch (type) {
        case MQTT_ACTIONS.messageReceived.type:
            return {
                ...state,
                messages: [
                    ...state.messages,
                    payload
                ]
            }
        case MQTT_ACTIONS.startingConnection.type:
            return {
                ...state,
                connectionStatus: CONNECTION_STATUS.CONNECTING
            }
        case MQTT_ACTIONS.connected.type:
            return {
                ...state,
                connectionStatus: CONNECTION_STATUS.CONNECTED
            }
        default:
            return state;
    }
}

export const MQTT_SELECTORS = {
    messages: (state) => state.messages,
    status: (state) => state.connectionStatus,
}
