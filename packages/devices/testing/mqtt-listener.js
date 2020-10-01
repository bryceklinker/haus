import {MQTT} from '../src/common/mqtt';

export class MqttListener {
    constructor() {
        this.client = null;
        this.receivedMessages = [];
        this.publishedMessages = [];
    }

    stop = async () => {
        if (this.client) {
            await this.client.end();
        }
    };

    subscribe = async () => {
        this.client = await MQTT.subscribe((topic, message) => {
            this.receivedMessages.push({
                topic, message: message.toString()
            });
        });
    };

    publish = async (topic, message) => {
        await MQTT.publish(topic, message);
    };
}
