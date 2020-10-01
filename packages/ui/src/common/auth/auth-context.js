import {Auth0Provider, useAuth0} from '@auth0/auth0-react';
import React from 'react';
import {Loading} from '../components/Loading';

export function useAuthUser() {
    const {user} = useAuth0();
    return user;
}

export function useLoginRedirect() {
    const {loginWithRedirect} = useAuth0();
    return () => loginWithRedirect();
}

export function useLogout() {
    const {logout} = useAuth0();
    return () => logout();
}

export function AuthProvider({settings, ...rest}) {
    if (!settings) {
        return <Loading size={'large'} />
    }
    return <Auth0Provider domain={settings.domain}
                          clientId={settings.client_id}
                          redirectUri={window.location.origin}
                          scope={settings.scope}
                          audience={settings.audience}
                          useRefreshTokens={true}
                          {...rest}/>
}
