import {bootstrapApp} from './app/bootstrapper';

const port = process.env.PORT || 3001;
bootstrapApp()
    .start(port)
    .catch(err => console.error(err));
