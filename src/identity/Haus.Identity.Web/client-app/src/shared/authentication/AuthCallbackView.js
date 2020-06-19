import React, {useEffect} from 'react';
import {useAuthService} from "./auth-hooks";
import {useHistory} from "react-router";
import {ROUTING_PATHS} from "../routing/routes";

export function AuthCallbackView() {
    const history = useHistory();
    const authService = useAuthService();
    authService.completeSignIn(window.location.href)
        .then(() => history.push(ROUTING_PATHS.WELCOME));
    
    
    return (
        <h3>Completing signin.</h3>
    )
}