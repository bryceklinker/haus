import {Redirect, Route, Switch} from "react-router";
import React from "react";

import {WelcomeView} from "../../welcome/views/WelcomeView";
import {ProtectedRoute} from "./ProtectedRoute";
import {AuthCallbackView} from "../authentication/AuthCallbackView";
import {UserListContainer} from '../../users/containers/UsersListContainer';

export const ROUTING_PATHS = {
    WELCOME: '/welcome',
    AUTH_CALLBACK: '/auth-callback',
    USERS: '/users'
};

export function Routes() {
    return (
        <Switch>
            <ProtectedRoute path={ROUTING_PATHS.WELCOME} component={WelcomeView} />
            <ProtectedRoute path={ROUTING_PATHS.USERS} component={UserListContainer} />
            <Route path={ROUTING_PATHS.AUTH_CALLBACK} component={AuthCallbackView}/>
            <Redirect to={ROUTING_PATHS.WELCOME} from={'/**'}/>
        </Switch>
    )
}