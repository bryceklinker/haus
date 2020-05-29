import {OIDC_CONFIGURATION} from "./oidc-configuration";
import {Provider} from 'oidc-provider';
import express from 'express';
import morgan from 'morgan'

let server, app = null;

export function start(port) {
    return new Promise((resolve, reject) => {
        app = express();
        app.use(morgan('tiny'));

        const oidcProvider = new Provider(`http://localhost:${port}`, OIDC_CONFIGURATION);
        app.use(oidcProvider.callback);

        server = app.listen(port, () => {
            console.log(`Now listening on port ${port}...`)
            resolve();
        });
        server.on('error', err => {
            console.error(err);
            reject();
        })
    })

}

export function stop() {
    if (server) {
        return new Promise((resolve) => {
            server.close(() => {
                console.log('Shutting down server');
                resolve();
            });
        })
    } else
    {
        return Promise.resolve();
    }
}


export default { start, stop };

