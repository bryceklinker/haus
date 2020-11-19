import React, {createContext, useContext, useMemo, useState} from 'react';
import {createTheme} from './create-theme';
import {CssBaseline, MuiThemeProvider, PaletteType, Theme} from '@material-ui/core';
import {THEME_TYPES} from './theme-types';

type ThemeContextValue = {
    theme: Theme;
    changeTheme: (type: PaletteType) => void;
}
const ThemeContext = createContext<ThemeContextValue>({
    theme: createTheme('dark'), changeTheme: (t) => {
    }
});

function useThemeContext(hookName: string) {
    const context = useContext(ThemeContext);
    if (context) {
        return context;
    }

    throw new Error(`${hookName} must be used under ${ThemeProvider.name}`);
}

function useChangeTheme() {
    const {changeTheme} = useThemeContext(useChangeTheme.name);
    return changeTheme;
}

function useThemeType() {
    const {theme} = useThemeContext(useThemeType.name);
    return theme.palette.type;
}

export function useIsDarkTheme() {
    return useThemeType() === THEME_TYPES.DARK;
}

export function useIsLightTheme() {
    return useThemeType() === THEME_TYPES.LIGHT;
}

export function useDarkTheme() {
    const changeTheme = useChangeTheme();
    return () => changeTheme(THEME_TYPES.DARK);
}

export function useLightTheme() {
    const changeTheme = useChangeTheme();
    return () => changeTheme(THEME_TYPES.LIGHT);
}

export function ThemeProvider({...rest}) {
    const [theme, setTheme] = useState(createTheme());
    const handleThemeChanged = (type: PaletteType) => setTheme(createTheme(type));
    const value = useMemo(() => ({theme, changeTheme: handleThemeChanged}), [theme]);
    return (
        <MuiThemeProvider theme={theme}>
            <CssBaseline/>
            <ThemeContext.Provider value={value} {...rest}/>
        </MuiThemeProvider>
    );

}
