import React, {createContext, useContext, useEffect, useMemo, useReducer, useState} from "react";
import {useSettings} from "../settings";
import {Loading} from "../loading";
import {AuthFailure} from "./AuthFailure";
import {authReducer, createInitialState, loadUserFailed, loadUserRequest, loadUserSuccess} from "./auth-state";
import {ensureUserSignedIn} from "./oidc-auth";

const AuthContext = createContext(null);

function useAuthContext() {
    const context = useContext(AuthContext);
    if (context) {
        return context;
    }
    
    throw new Error(`${Function.name} must be used within AuthContext`)
}

export function useCurrentUser() {
    const [state, dispatch] = useAuthContext();
    useEffect(() => {
        if (state.isLoading || state.user) {
            return;
        }
        
        dispatch(loadUserRequest());
        ensureUserSignedIn(state.userManager)
            .then(user => dispatch(loadUserSuccess(user)))
            .catch(err => dispatch(loadUserFailed(err)));
    }, []);
    
    return state.user;
}

export function AuthProvider(props) {
    const settings = useSettings();
    const [state, dispatch] = useReducer(authReducer, createInitialState(settings))
    const value = useMemo(() => [state, dispatch], [state]);
    return (
        <AuthContext.Provider value={value} {...props}/>
    )
}