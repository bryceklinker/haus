import React from 'react';
import {useAuthService} from "./auth-hooks";
import {useHistory} from "react-router";
import {ROUTING_PATHS} from "../routing/routes";
import {LoadingView} from '../loading/LoadingView';

export function AuthCallbackView() {
    const history = useHistory();
    const authService = useAuthService();
    authService.completeSignIn()
        .then(() => history.push(ROUTING_PATHS.WELCOME));
    
    return (
        <div>
            <h3>Completing signin.</h3>
            <LoadingView show={true} />
        </div>
        
    )
}