import React, {useState} from "react";
import {createMuiTheme, ThemeProvider} from '@material-ui/core'
import orange from "@material-ui/core/colors/orange";
import yellow from "@material-ui/core/colors/yellow";
import TextField from "@material-ui/core/TextField";
import Button from "@material-ui/core/Button";
import CssBaseline from "@material-ui/core/CssBaseline";

const theme = createMuiTheme({
    palette: {
        primary: orange,
        secondary: yellow,
        type: 'dark'
    }
});

export function Login() {
    const [state, setState] = useState({username: null, password: null});
    const createChangeHandler = (field) => {
        return (evt) => setState({...state, [field]: evt.target.value});
    };
    return (
        <ThemeProvider theme={theme}>
            <CssBaseline />
            <form method={'POST'} action={'/users/login'} >
                <TextField data-testid={'username-input'} onChange={createChangeHandler('username')} />
                <TextField data-testid={'password-input'} onChange={createChangeHandler('password')} />
                <Button variant={'contained'} color={'primary'} type={'submit'}>
                    Login
                </Button>
            </form>
        </ThemeProvider>
        
    )
}