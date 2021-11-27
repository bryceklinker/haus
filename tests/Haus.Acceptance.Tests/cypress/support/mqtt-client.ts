import {Zigbee2MqttMessage} from "./zigbee-2-mqtt-message-builder";

function publish<T>(topic: string, data: T) {
    const json = JSON.stringify(data);
    return cy.task('publishToMqtt', {topic, json});
}

function publishZigbee2MqttMessage({topic, ...data}: Zigbee2MqttMessage) {
    return publish(topic, data);
}

export const MQTT_CLIENT = {
    publish,
    publishZigbee2MqttMessage
}