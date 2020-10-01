import settings from 'zigbee2mqtt/lib/util/settings';

export const config = {
    mqtt_url: settings.get().mqtt.server,
    mqtt_base_topic: settings.get().mqtt.base_topic
}
