import {createMuiTheme} from '@material-ui/core';
import {orange, yellow} from '@material-ui/core/colors';
import {THEME_TYPES, ThemeType} from './theme-types';

export function createTheme(type: ThemeType = THEME_TYPES.DARK) {
    return createMuiTheme({
        palette: {
            primary: orange,
            secondary: yellow,
            type
        }
    });
}
