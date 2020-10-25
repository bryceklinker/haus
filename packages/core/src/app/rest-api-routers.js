import {createDiscoveryRouter} from '../discovery/discovery-router';

export const REST_API_ROUTERS = [
    {path: '/discovery', factory: createDiscoveryRouter}
];
