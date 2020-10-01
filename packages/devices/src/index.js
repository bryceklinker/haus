import path from 'path';

require('dotenv').config({
    path: path.resolve(__dirname, '..', '.env')
});

const server = require('./server').createDevicesServer();
server.start()
    .catch(err => console.error(err));
