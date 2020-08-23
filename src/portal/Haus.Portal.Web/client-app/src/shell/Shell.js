import React from "react";
import {ThemeProvider} from '@material-ui/core';

import {SettingsProvider} from "../shared/settings";
import {darkTheme} from "../shared/theming";
import {AuthProvider} from "../shared/auth";
import {Header} from "./Header";
import {Main} from "./Main";

export function Shell() {
    return (
        <ThemeProvider theme={darkTheme}>
            <SettingsProvider>
                <AuthProvider>
                    <div>
                        <Header />
                        <Main />
                    </div>
                </AuthProvider>
            </SettingsProvider>    
        </ThemeProvider>
        
    )
}