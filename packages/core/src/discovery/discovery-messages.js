function start() {
    return {type: DISCOVERY_MESSAGES.TYPES.START};
}

function stop() {
    return {type: DISCOVERY_MESSAGES.TYPES.STOP};
}

export const DISCOVERY_MESSAGES = {
    TYPES: {
        START: 'discovery/start',
        STOP: 'discovery/stop'
    },
    start,
    stop
};
