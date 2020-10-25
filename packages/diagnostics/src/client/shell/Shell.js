import React from 'react';
import {MqttProvider} from '../common/mqtt/mqtt.context';
import {Header} from './Header';
import {MainContent} from './MainContent';
import {Footer} from './Footer';
import {ThemeProvider} from '../common/theming/theme.context';

export function Shell() {
    return (
        <ThemeProvider>
            <MqttProvider>
                <div>
                    <Header/>
                    <MainContent/>
                    <Footer/>
                </div>
            </MqttProvider>
        </ThemeProvider>
    );
}
