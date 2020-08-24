import React, {createContext, useContext, useEffect, useState} from "react";
import {useSettings} from "../settings";
import {Loading} from "../loading";
import {createUserManager, ensureUserIsAuthenticated, AUTH_STATE} from "./auth-handler";
import {AuthFailure} from "./AuthFailure";

const AuthContext = createContext(null);

export function useCurrentUser() {
    const context = useContext(AuthContext);
    if (context) {
        return context;
    }

    throw new Error(`${Function.name} must be used under AuthProvider`);
}

function AuthView({children, state}) {
    if (state === AUTH_STATE.AUTHENTICATED) {
        return children
    }
    
    if (state === AUTH_STATE.ERROR) {
        return <AuthFailure />;
    }
    
    return <Loading />;
}

export function AuthProvider({children, ...rest}) {
    const settings = useSettings();
    const [userManager] = useState(createUserManager(settings));
    const [auth, setAuth] = useState({state: AUTH_STATE.UNAUTHENTICATED, user: null});
    useEffect(() => {
        ensureUserIsAuthenticated(userManager, settings.responseType)
            .then(value => setAuth(value))
            .catch(() => userManager.signinRedirect());
    });
    return (
        <AuthContext.Provider value={auth.user} {...rest}>
            <AuthView auth={auth.state} {...rest}>
                {children}
            </AuthView>
        </AuthContext.Provider>
    )
}