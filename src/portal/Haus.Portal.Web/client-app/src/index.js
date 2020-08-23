import React from 'react';
import {render} from 'react-dom';
import * as serviceWorker from './serviceWorker';
import {Shell} from "./shell/Shell";

render(
  <React.StrictMode>
    <Shell />
  </React.StrictMode>,
  document.getElementById('root')
);

serviceWorker.unregister();
