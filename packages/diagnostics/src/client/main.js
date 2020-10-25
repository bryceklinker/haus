import React from 'react';
import {render} from 'react-dom';
import {Shell} from './shell/Shell';
import {Buffer} from 'buffer';

window.Buffer = Buffer;
render(
    <Shell />,
    document.getElementById('react-root')
);
