import express from 'express';
import {webpack} from 'webpack';
import createConfig from '../../webpack.config';
import middleware from 'webpack-dev-middleware';

export function createExpressApp() {
    const app = express();
    const compiler = webpack(createConfig('', {mode: 'development'}));
    app.use(middleware(compiler, {}));
    return app;
}
