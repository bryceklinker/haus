import React, {createContext, useContext, useState} from 'react';
import {createMuiTheme, MuiThemeProvider} from '@material-ui/core';
import {orange, yellow} from '@material-ui/core/colors';
import CssBaseline from '@material-ui/core/CssBaseline';

const THEME_TYPES = {
    LIGHT: 'light',
    DARK: 'dark'
};

function createTheme(type = THEME_TYPES.DARK) {
    return createMuiTheme({
        palette: {
            primary: orange,
            secondary: yellow,
            type: type
        }
    });
}

const ThemeContext = createContext(createTheme());

function useThemeContext(hookName) {
    const context = useContext(ThemeContext);
    if (context) {
        return context;
    }

    throw new Error(`${hookName} must be used under ${ThemeProvider.name}`);
}

export function useChangeTheme() {
    const [, handleChangeTheme] = useThemeContext(useChangeTheme.name);

    return handleChangeTheme;
}

export function useThemeTypes() {
    return THEME_TYPES;
}

export function useThemeType() {
    const [theme] = useThemeContext(useThemeType.name);
    return theme.palette.type;
}

export function ThemeProvider({...rest}) {
    const [theme, setTheme] = useState(createTheme());
    const handleTypeChange = (newType) => setTheme(createTheme(newType));

    return (
        <MuiThemeProvider theme={theme}>
            <CssBaseline />
            <ThemeContext.Provider value={[theme, handleTypeChange]} {...rest} />
        </MuiThemeProvider>
    );
}
