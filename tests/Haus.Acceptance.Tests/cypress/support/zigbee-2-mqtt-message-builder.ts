import * as path from 'path';
import {DeviceValues, Zigbee2MqttMessage, Zigbee2MqttMeta} from './models';

const BASE_TOPIC = 'zigbee2mqtt';
const TOPICS = {
    STATE: path.join(BASE_TOPIC, 'bridge', 'state'),
    LOG: path.join(BASE_TOPIC, 'bridge', 'log'),
    CONFIG: path.join(BASE_TOPIC, 'bridge', 'config'),
    DEVICES: path.join(BASE_TOPIC, 'bridge', 'config', 'devices')
};
const DEFAULT_ZIGBEE_2_MQTT_MESSAGE: Zigbee2MqttMessage = {
    topic: 'zigbee2mqtt',
};

export class Zigbee2MqttMessageBuilder {
    private _current: Zigbee2MqttMessage;

    constructor() {
        this._current = Zigbee2MqttMessageBuilder.createDefault();
    }

    withTopic(topic: string): Zigbee2MqttMessageBuilder {
        this._current = {...this._current, topic};
        return this;
    }

    withStateTopic(): Zigbee2MqttMessageBuilder {
        return this.withTopic(TOPICS.STATE);
    }

    withDeviceTopic(name: string): Zigbee2MqttMessageBuilder {
        return this.withTopic(path.join(TOPICS.DEVICES, name));
    }

    withLogTopic(): Zigbee2MqttMessageBuilder {
        return this.withTopic(TOPICS.LOG);
    }

    withType(type: string): Zigbee2MqttMessageBuilder {
        this._current = {...this._current, type};
        return this;
    }

    withPairing(): Zigbee2MqttMessageBuilder {
        return this.withType('pairing');
    }

    withMessage(message: string): Zigbee2MqttMessageBuilder {
        this._current = {...this._current, message};
        return this;
    }

    withInterviewSuccessful(): Zigbee2MqttMessageBuilder {
        return this.withMessage('interview_successful');
    }

    withValue<Key extends keyof DeviceValues, Value extends DeviceValues[Key]>(key: Key, value: Value) {
        this._current = {...this._current, [key]: value};
        return this;
    }

    withMeta(meta: Partial<Zigbee2MqttMeta>): Zigbee2MqttMessageBuilder {
        this._current = {
            ...this._current,
            meta: {
                ...meta
            }
        };
        return this;
    }

    withPhillipsLightMeta(friendlyName: string): Zigbee2MqttMessageBuilder {
        return this.withMeta({
            friendly_name: friendlyName,
            vendor: 'Philips',
            model: '929002335001'
        });
    }

    withPhillipsMotionSensor(friendlyName: string): Zigbee2MqttMessageBuilder {
        return this.withMeta({
            friendly_name: friendlyName,
            vendor: 'Philips',
            model: '9290012607'
        });
    }

    build(): Zigbee2MqttMessage {
        try {
            return this._current;
        } finally {
            this._current = Zigbee2MqttMessageBuilder.createDefault();
        }
    }

    private static createDefault() {
        return {...DEFAULT_ZIGBEE_2_MQTT_MESSAGE};
    }
}