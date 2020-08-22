import path from 'path';

const REPO_ROOT = path.resolve(path.join(__dirname, '..', '..'))
const SRC_ROOT = path.join(REPO_ROOT, 'src');

export const IDENTITY_OPTIONS = {
    name: 'identity',
    projectPath: path.join(SRC_ROOT, 'identity', 'Haus.Identity.Web'),
    httpsPort: 5003,
    httpPort: 5002
}

export const PORTAL_OPTIONS = {
    name: 'portal',
    projectPath: path.join(SRC_ROOT, 'portal', 'Haus.Portal.Web'),
    httpsPort: 5001,
    httpPort: 5000
}

export const ALL = [
    IDENTITY_OPTIONS,
    PORTAL_OPTIONS
]