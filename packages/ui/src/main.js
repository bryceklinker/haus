import React from 'react';
import {render} from 'react-dom';
import {SettingsLoader} from './common';
import {Shell} from './shell/components';

async function main() {
    const settings = await SettingsLoader.load();
    render(
        <Shell settings={settings} />,
        document.getElementById('root')
    )
}

main()
    .catch(err => console.error(err));
