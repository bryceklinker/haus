import React from "react";
import Button from "@material-ui/core/Button";
import {useInput} from "../../../shared/hooks/use-input";
import {TextInput} from "../../../shared/components/TextInput";

export function LoginView({onLogin}) {
    const [username, usernameHandler] = useInput();
    const [password, passwordHandler] = useInput();
    const loginHandler = (evt) => {
        if (evt) {
            evt.preventDefault();    
        }
        onLogin({ username, password });
    }
    return (
        <form data-testid={'login-form'} onSubmit={loginHandler}>
            <TextInput data-testid={'username-input'} value={username} onChange={usernameHandler} />
            <TextInput data-testid={'password-input'} value={password} onChange={passwordHandler} type={'password'} />
            <Button data-testid={'login-button'} type={'submit'} onClick={loginHandler}>
                Login
            </Button>
        </form>
    )
}