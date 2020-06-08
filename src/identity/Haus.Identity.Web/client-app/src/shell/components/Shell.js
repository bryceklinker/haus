import React from "react";
import {Redirect, Route, Switch} from "react-router";

import {Footer} from "./Footer";
import {MainContent} from "./MainContent";
import {Header} from "./Header";
import {WelcomeView} from "../../welcome/views/WelcomeView";
import {LoginContainer} from "../../account/login/containers/LoginContainer";

function Routes() {
    return (
        <Switch>
            <Route path={'/account/login'} component={LoginContainer} />
            <Route path={'/welcome'} component={WelcomeView} />
            <Redirect to={'/account/login'} from={'/**'} />
        </Switch>
    )
}

export function Shell({}) {
    return (
        <div>
            <Header />
            
            <MainContent>
                <Routes />
            </MainContent>

            <Footer />    
        </div>
    )
}