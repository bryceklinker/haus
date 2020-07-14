import React, {createContext, useContext, useEffect, useState} from 'react';
import {AuthService} from "./auth-service";

const INITIAL_AUTH_SERVICE = {
    isSignedIn: () => false,
    startSignIn: () => Promise.resolve(),
    completeSignIn: () => Promise.reject('Not Ready')
}

const AuthContext = createContext(INITIAL_AUTH_SERVICE);

export function useAuthService() {
    const context = useContext(AuthContext);
    if (context === undefined) {
        throw new Error('useAuthService must be used under AuthProvider');
    }
    return context;
}

function useAuth() {
    const [settings, setSettings] = useState(null);
    
    useEffect(() => {
        async function loadSettings() {
            const response = await fetch('/api/settings');
            setSettings(await response.json());
        }
        
        loadSettings().catch(err => console.error(`Failed to load settings.json: ${err}`));
    }, []);
    
    if (settings) {
        return new AuthService(settings);
    } else {
        return INITIAL_AUTH_SERVICE;
    }
}

export function AuthProvider({children}) {
    const auth = useAuth();
    return (
        <AuthContext.Provider value={auth}>
            {children}
        </AuthContext.Provider>
    )
}