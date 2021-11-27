import {Zigbee2MqttMessage, Zigbee2MqttMessageBuilder} from "./zigbee-2-mqtt-message-builder";

function createLightDiscoveredMessage(friendlyName: string): Zigbee2MqttMessage {
    return new Zigbee2MqttMessageBuilder()
        .withLogTopic()
        .withPairing()
        .withInterviewSuccessful()
        .withPhillipsLightMeta(friendlyName)
        .build();
}

function createMotionSensorDiscoveredMessage(friendlyName: string): Zigbee2MqttMessage {
    return new Zigbee2MqttMessageBuilder()
        .withLogTopic()
        .withPairing()
        .withInterviewSuccessful()
        .withPhillipsMotionSensor(friendlyName)
        .build();
}

export const Zigbee2MqttMessageFactory = {
    createLightDiscoveredMessage,
    createMotionSensorDiscoveredMessage
}