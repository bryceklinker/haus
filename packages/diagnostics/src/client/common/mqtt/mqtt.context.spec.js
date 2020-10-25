import React from 'react';
import {act, render, waitFor} from '@testing-library/react';
import {MqttProvider, useMqttConnectionStatus, useMqttMessages} from './mqtt.context';
import {FakeMqttClient} from '../../../../testing/fake-mqtt-client';

const MQTT_URL = 'mqtt://something.com';

describe('MqttContext', () => {
    let connectFake, fakeMqttClient;

    beforeEach(() => {
        fakeMqttClient = new FakeMqttClient();
        connectFake = jest.fn().mockReturnValue(fakeMqttClient);
    });

    test('when rendered then shows connection state', () => {
        const {queryByTestId} = renderMqttProvider();

        expect(queryByTestId('status')).toHaveTextContent('connecting');
    });

    test('when rendered then mqtt connection is started', async () => {
        const {queryByTestId} = renderMqttProvider();

        act(() => {
            fakeMqttClient.emit('connect');
        });

        await waitFor(() => {
            expect(queryByTestId('loading')).not.toBeInTheDocument();
        });
    });

    test('when mqtt receives a message then messages is updated', async () => {
        const {queryByTestId} = renderMqttProvider();

        act(() => {
            fakeMqttClient.emit('connect');
            fakeMqttClient.emitMessage({data: 'data'});
        });

        await waitFor(() => {
            expect(queryByTestId('message')).toBeInTheDocument();
        });
    });

    function renderMqttProvider() {
        return render(
            <MqttProvider connect={connectFake} mqttUrl={MQTT_URL}>
                <MqttContextTesting/>
            </MqttProvider>
        );
    }
});

function MqttContextTesting() {
    const messages = useMqttMessages();
    const status = useMqttConnectionStatus();
    return (
        <div>
            <h3 data-testid={'status'}>{status}</h3>
            <ul>
                {
                    messages.map((m, i) => (
                        <li data-testid={'message'} key={i}>
                            <pre>
                                {JSON.stringify(m, null, 2)}
                            </pre>
                        </li>
                    ))
                }
            </ul>
        </div>
    );
}
