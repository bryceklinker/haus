import MQTT_CLIENT from './mqtt-client';

const EVENT_TOPIC = 'haus/events';

let handlers = {};
let hasSubscribed = false;

async function subscribe(type, handler) {
    await ensureSubscribed();

    handlers[type] = [...(handlers[type] || []), handler];
}

export const EVENT_BUS = {subscribe};
export default EVENT_BUS;

async function ensureSubscribed() {
    if (hasSubscribed) {
        return;
    }

    const client = await MQTT_CLIENT.getClient();
    client.subscribe(EVENT_TOPIC);
    client.on('message', async (topic, buffer) => {
        const message = JSON.parse(buffer.toString());
        const messageHandlers = handlers[message.type] || [];
        const promises = messageHandlers.map(async h => await h(message));
        await Promise.all(promises);
    });
    hasSubscribed = true;
}
