import React from 'react';
import {Auth0Provider} from '@auth0/auth0-react';
import {ThemeProvider} from '../../common';
import {BrowserRouter} from 'react-router-dom';
import CssBaseline from '@material-ui/core/CssBaseline';
import {Header} from './Header';
import {Main} from './Main';
import makeStyles from '@material-ui/core/styles/makeStyles';
import {AuthProvider} from '../../common/auth';

const useStyles = makeStyles(theme => ({
    root: {
        flex: 1,
        display: 'flex'
    },
    offset: theme.mixins.toolbar
}));

export function Shell({settings}) {
    const classes = useStyles();
    return (
        <AuthProvider settings={settings}>
            <ThemeProvider>
                <BrowserRouter>
                    <div className={classes.root}>
                        <CssBaseline />
                        <Header />
                        <div className={classes.offset}/>
                        <Main />
                    </div>
                </BrowserRouter>
            </ThemeProvider>
        </AuthProvider>
    );
}
