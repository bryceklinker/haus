import {mqttReducer} from '../src/client/common/mqtt/mqtt.reducer';
import {MqttContext} from '../src/client/common/mqtt/mqtt.context';
import React from 'react';

export function TestingMqttProvider({actions = [], ...rest}) {
    const state = actions.reduce((state, action) => mqttReducer(state, action), mqttReducer());
    return <MqttContext.Provider value={[state]}  {...rest} />
}
