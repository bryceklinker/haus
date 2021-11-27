export type Optional<T> = T | null | undefined;

export type Zigbee2MqttMeta = {
    description?: Optional<string>;
    friendly_name?: Optional<string>;
    model?: Optional<string>;
    vendor?: Optional<string>;
    supported?: Optional<boolean>;
}

export type Zigbee2MqttMessage = {
    topic: string;
    type?: Optional<string>;
    message?: Optional<string>;
    battery?: Optional<number>;
    illuminance?: Optional<number>;
    illuminance_lux?: Optional<number>;
    linkquality?: Optional<number>;
    motion_sensitivity?: Optional<string>;
    occupancy?: Optional<boolean>;
    occupancy_timeout?: Optional<number>;
    temperature?: Optional<number>;
    meta?: Optional<Zigbee2MqttMeta>;
}

export type DeviceValues = Omit<Zigbee2MqttMessage, 'topic' | 'type' | 'message' | 'meta'>;

export interface TokenResponse {
    id_token: string;
    access_token: string;
}