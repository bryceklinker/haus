import React, {createContext, useReducer, useMemo, useContext} from 'react';
import {createMuiTheme, ThemeProvider as MaterialThemeProvider} from '@material-ui/core';
import {blue, orange} from '@material-ui/core/colors';

function createTheme(type = 'dark') {
    return createMuiTheme({
        palette: {
            primary: orange,
            secondary: blue,
            type
        }
    });
}

export const THEME_TYPES = {
    LIGHT: 'light',
    DARK: 'dark',
    ALL: ['light', 'dark']
}

const ThemeContext = createContext();

function useTheme() {
    const context = useContext(ThemeContext);
    if (context)
        return context;

    throw new Error(`${useTheme.name} must be used under ${ThemeProvider.name}.`);
}

export function useSetTheme() {
    const [, dispatch] = useTheme();
    return type => dispatch({
        type: 'change',
        payload: type
    });
}

export function useThemeType() {
    const [state] = useTheme();
    return state.theme.palette.type;
}

function reducer(state, action) {
    switch (action.type) {
        case 'change':
            return {theme: createTheme(action.payload)};
        default:
            return state;
    }
}

export function ThemeProvider({children, ...rest}) {
    const [state, dispatch] = useReducer(reducer, {theme: createTheme()});
    const value = useMemo(() => [state, dispatch], [state]);
    return (
        <ThemeContext.Provider value={value} {...rest}>
            <MaterialThemeProvider theme={state.theme}>
                {children}
            </MaterialThemeProvider>
        </ThemeContext.Provider>
    );
}
