import path from 'path';

export const IDENTITY_OPTIONS = {
    name: 'identity',
    projectPath: path.join('src', 'identity', 'Haus.Identity.Web'),
    httpsPort: 5003,
    httpPort: 5002
}

export const PORTAL_OPTIONS = {
    name: 'portal',
    projectPath: path.join('src', 'portal', 'Haus.Portal.Web'),
    httpsPort: 5001,
    httpPort: 5000
}

export const ALL_WEB_APPS = [
    IDENTITY_OPTIONS,
    PORTAL_OPTIONS
]