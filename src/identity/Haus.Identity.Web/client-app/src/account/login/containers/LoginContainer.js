import React from 'react';
import {useHistory} from "react-router";
import {LoginView} from "../views/LoginView";
import {parseQueryString} from "../../../shared/parsers/query-string-parser";
import {createLoginRequest} from "../factories/login-request-factory";

function createLoginHandler(history, redirectUrl) {
    return async (credentials) => {
        const requestData = createLoginRequest(credentials);
        const response = await fetch('/api/account/login', {
            method: 'POST',
            body: requestData
        });

        if (response.ok) {
            const json = await response.json();
            history.push(`${redirectUrl}`);
        }
    }
}

export function LoginContainer() {
    const history = useHistory();
    const queryParameters = parseQueryString(window.location.href);
    const loginHandler = createLoginHandler(history, queryParameters.get('redirect_url'));
    return <LoginView onLogin={loginHandler}
                      redirectUrl={queryParameters.get('redirect_url')}
                      clientId={queryParameters.get('client_id')}
                      scope={queryParameters.get('scope')} />
}