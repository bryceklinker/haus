import {Zigbee2MqttMessage, Zigbee2MqttMessageBuilder} from "./zigbee-2-mqtt-message-builder";

function interviewSuccessful(friendlyName: string): Zigbee2MqttMessage {
    return new Zigbee2MqttMessageBuilder()
        .withLogTopic()
        .withPairing()
        .withInterviewSuccessful()
        .withPhillipsLightMeta(friendlyName)
        .build();
}

export const Zigbee2MqttMessageFactory = {
    interviewSuccessful
}