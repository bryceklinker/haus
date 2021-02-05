interface InterceptorConfig {
    name: string;
    url: string;
    alias: string;
}
export const INTERCEPTORS = {
    settings: {
        name: 'settings',
        url: '/client-settings',
        alias: '@settings'
    } as InterceptorConfig
}