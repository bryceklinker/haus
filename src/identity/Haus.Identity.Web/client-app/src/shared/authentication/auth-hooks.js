import React, {createContext, useContext, useReducer} from 'react';
import {AuthService} from "./auth-service";

const service = new AuthService();
const initialState = {
    service: service,
    user: service.getUser()
}
const AuthContext = createContext();

const authReducer = (state, action) => {
    switch (action.type()) {
        case 'updateUser':
            return {
                ...state,
                user: action.payload
            }
        default:
            return state;
    }
}

export function useAuthService() {
    const context = useContext(AuthContext);
    if (context === undefined) {
        throw new Error('useAuthService must be used under AuthProvider');
    }
    return context.service;
}

export function useAuthUser() {
    const context = useContext(AuthContext);
    if (context === undefined) {
        throw new Error('useAuthUser must be used under AuthProvider');
    }
    return context.user;
}

export function AuthProvider({children}) {
    const [state, dispatch] = useReducer(authReducer, initialState);
    return (
        <AuthContext.Provider value={{
            ...state,
            updateUser: (user) => dispatch({ type: 'updateUser', payload: user })
        }}>
            {children}
        </AuthContext.Provider>
    )
}