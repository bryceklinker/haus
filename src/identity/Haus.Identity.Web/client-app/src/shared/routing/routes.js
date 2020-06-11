import {Redirect, Route, Switch} from "react-router";
import {WelcomeView} from "../../welcome/views/WelcomeView";
import React from "react";

export const ROUTING = {
    welcome: {
        path: '/welcome',
        component: WelcomeView
    }
};

export function Routes() {
    const routes = Object.keys(ROUTING)
        .filter(k => ROUTING.hasOwnProperty(k))
        .map(k => ROUTING[k])
        .map(r => <Route {...r} />);
    return (
        <Switch>
            {routes}
            <Redirect to={'/welcome'} from={'/**'} />
        </Switch>
    )
}