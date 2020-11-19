import * as React from 'react';
import {render} from 'react-dom';
import {MessagesProvider} from './messages/context/messages.context';
import {App} from './App';
import {ThemeProvider} from './common/theming/theme.context';

import './index.scss';
render(
    <MessagesProvider>
        <ThemeProvider>
            <App/>
        </ThemeProvider>
    </MessagesProvider>,
    document.getElementById('root')
);
