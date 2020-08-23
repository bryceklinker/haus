import React, {createContext, useContext, useEffect, useState} from "react";
import {useSettings} from "../settings";
import {UserManager} from "oidc-client";
import {Loading} from "../loading";

const AuthContext = createContext(null);

export function useCurrentUser() {
    const context = useContext(AuthContext);
    if (context) {
        return context;
    }
    
    throw new Error(`${Function.name} must be used under AuthProvider`);
}

function isCallback(responseType) {
    const params = new URLSearchParams(window.location.query);
    return params.has(responseType);
}

async function ensureUserIsAuthenticated(userManager, responseType) {
    const user = isCallback(responseType)
        ? await userManager.signinRedirectCallback()
        : await userManager.getUser();
    
    if (user) {
        return user;
    }
    
    throw new Error('User is not authenticated');
}

export function AuthProvider({children, ...rest}) {
    const settings = useSettings();
    const [userManager] = useState(new UserManager({
        authority: settings.authority,
        client_id: settings.clientId,
        response_type: settings.responseType,
        redirect_uri: window.origin
    }));
    const [user, setUser] = useState(null);
    useEffect(() => {
        ensureUserIsAuthenticated(userManager, settings.responseType)
            .then(user => setUser(user))
            .catch(() => userManager.signinRedirect());
    })
    return (
        <AuthContext.Provider value={user}>
            {user ? children : <Loading />}
        </AuthContext.Provider>
    )
}