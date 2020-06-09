import React from 'react';
import {useHistory} from "react-router";
import {LoginView} from "../views/LoginView";

export function LoginContainer() {
    const history = useHistory();
    const loginHandler = async (credentials) => {
        const response = await fetch('/api/account/login', {
            method: 'POST',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(credentials)
        });
        
        if (response.ok) {
            history.push('/welcome');
        }
    };
    return <LoginView onLogin={loginHandler}/>
}