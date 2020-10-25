import io from 'socket.io-client';
import React, {createContext, useContext, useEffect, useMemo, useReducer} from 'react';
import {initialState, MQTT_SELECTORS, mqttReducer} from './mqtt.reducer';
import {MQTT_ACTIONS} from './actions';

export const MqttContext = createContext(null);

function useMqttContext(hookName) {
    const context = useContext(MqttContext);
    if (context) {
        return context;
    }

    throw new Error(`${hookName} must be used under ${MqttProvider.name}`);
}

export function useMqttMessages() {
    const [state] = useMqttContext(useMqttMessages.name);
    return MQTT_SELECTORS.messages(state);
}

export function useMqttConnectionStatus() {
    const [state] = useMqttContext(useMqttConnectionStatus.name);
    return MQTT_SELECTORS.status(state);
}

export function MqttProvider({connect = io, ...rest}) {
    const [state, dispatch] = useReducer(mqttReducer, initialState);
    const value = useMemo(() => [state, dispatch], [state]);
    useEffect(() => {
        dispatch(MQTT_ACTIONS.startingConnection());
        const socket = connect();
        socket.on('connect', () => {
            dispatch(MQTT_ACTIONS.connected());

            socket.on('message', (data) => {
                dispatch(MQTT_ACTIONS.messageReceived(data));
            });
        });
    }, [connect]);

    return <MqttContext.Provider value={value} {...rest} />;
}
