import React from 'react';
import {Route} from "react-router";
import {useAuthService} from "../authentication/auth-hooks";


export function ProtectedRoute({component: Component, ...rest}) {
    const authService = useAuthService();
    return (
        <Route {...rest} render={props => {
            if (authService.isSignedIn()) {
                return (
                    <Component {...props} />
                )
            }
            authService.startSignIn();
            return null;
        }} />
)
}