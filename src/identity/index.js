const { Provider } = require('oidc-provider');
const express = require('express');
const morgan = require('morgan');

const PORT = process.env.PORT || 3000;
const app = express();
app.use(morgan('tiny'));

const oidc = new Provider(`http://localhost:${PORT}`, {});
app.use(oidc.callback);

app.listen(PORT, () => {
    console.log(`Now listening on port ${PORT}...`)
});

