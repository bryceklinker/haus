import {createMuiTheme} from "@material-ui/core";
import {orange, yellow} from "@material-ui/core/colors";

export const darkTheme = createMuiTheme({
    palette: {
        primary: orange,
        secondary: yellow,
        type: 'dark'
    }
})