import {mqttReducer} from './mqtt.reducer';
import {MQTT_ACTIONS} from './actions';

describe('mqttReducer', () => {
    test('when initialized then returns default state', () => {
        const state = mqttReducer();

        expect(state).toEqual({
            connectionStatus: 'not connected',
            messages: []
        });
    });

    test('when message received then returns state with one message', () => {
        const message = {something: 'goes here'};
        let state = mqttReducer();
        state = mqttReducer(state, MQTT_ACTIONS.messageReceived(message));

        expect(state.messages).toContainEqual(message);
    });

    test('when starting connection received then returns connecting state', () => {
        let state = mqttReducer();
        state = mqttReducer(state, MQTT_ACTIONS.startingConnection());

        expect(state.connectionStatus).toEqual('connecting');
    });

    test('when connected received then returns connected state', () => {
        let state = mqttReducer();
        state = mqttReducer(state, MQTT_ACTIONS.connected());

        expect(state.connectionStatus).toEqual('connected');
    });
});
