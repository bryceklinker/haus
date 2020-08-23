import React from "react";
import {TextInput, useInput, Main, ContainedButton} from "../shared";
import {makeStyles} from "@material-ui/core";

const useStyles = makeStyles(theme => ({
    loginButton: {
        marginTop: theme.spacing(1),
        marginLeft: theme.spacing(1)
    }
}))

export function Login() {
    const [, bindUsername] = useInput(initialUsername || '');
    const [, bindPassword] = useInput();
    const classes = useStyles();
    return (
        <Main>
            <form action={'/users/login'} method={'POST'}>
                <TextInput data-testid={'username-input'}
                           fullWidth
                           label={'Username'}
                           name={'username'}
                           placeholder={'Username'}
                           {...bindUsername} />
                <TextInput data-testid={'password-input'}
                           fullWidth
                           label={'Password'}
                           type={'password'}
                           name={'password'}
                           placeholder={'Password'}
                           {...bindPassword} />
                <ContainedButton className={classes.loginButton} type={'submit'} color={'primary'}>
                    Login
                </ContainedButton>
            </form>
        </Main>
    )
}