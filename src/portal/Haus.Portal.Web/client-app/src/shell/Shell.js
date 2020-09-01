import React from "react";
import {ThemeProvider, CssBaseline, makeStyles} from '@material-ui/core';

import {SettingsProvider} from "../shared/settings";
import {darkTheme} from "../shared/theming";
import {AuthProvider, useCurrentUser} from "../shared/auth";
import {Header} from "./Header";
import {Main} from "./Main";
import {Loading} from "../shared/loading";

const useStyles = makeStyles(theme => ({
    spacer: theme.mixins.toolbar
}))
function ShellView() {
    const classes = useStyles();
    const user = useCurrentUser();
    if (!user) {
        return <Loading />;
    }
    return (
        <div>
            <Header />
            <div className={classes.spacer} />
            <Main />
        </div>
    );
}

export function Shell() {
    return (
        <ThemeProvider theme={darkTheme}>
            <CssBaseline />
            <SettingsProvider>
                <AuthProvider>
                    <ShellView />
                </AuthProvider>
            </SettingsProvider>    
        </ThemeProvider>
        
    )
}