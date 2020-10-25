import {useMqttMessages} from '../common/mqtt/mqtt.context';
import React from 'react';

export function Messages() {
    const messages = useMqttMessages();
    return (
        <div>

        </div>
    )
}
