import {createMuiTheme, } from '@material-ui/core';
import orange from '@material-ui/core/colors/orange';
import yellow from '@material-ui/core/colors/yellow'

export function createTheme(isDark) {
    return createMuiTheme({
        palette: {
            primary: orange,
            secondary: yellow,
            type: isDark ? 'dark' : 'light'
        }
    });
}