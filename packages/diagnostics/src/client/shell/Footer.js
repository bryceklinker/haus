import React from 'react';
import {Typography} from '@material-ui/core';
import {useMqttConnectionStatus} from '../common/mqtt/mqtt.context';

export function Footer() {
    const connectionStatus = useMqttConnectionStatus();
    return (
        <footer>
            <Typography variant={'h6'} gutterBottom>
                Connection Status: {connectionStatus}
            </Typography>
        </footer>
    )
}
