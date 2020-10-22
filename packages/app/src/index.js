const {bootstrapApp} = require('@haus/core');

const port = process.env.PORT || 5000;
const settings = {
    logLevel: 'info',
    connectionString: 'sqlite::memory:'
}
bootstrapApp(settings)
    .start(port)
    .catch(err => console.error(err));
