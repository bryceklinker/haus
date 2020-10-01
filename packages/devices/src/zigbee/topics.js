import {config} from '../common/config';

const BASE_NAME = `${config.mqtt_base_topic}/bridge`;

// zigbee2mqtt messages: https://www.zigbee2mqtt.io/information/mqtt_topics_and_message_structure.html
export const ZIGBEE_TOPICS = {
    PERMIT_JOIN: `${BASE_NAME}/config/permit_join`,
    LOG: `${BASE_NAME}/log`,
    STATE: `${BASE_NAME}/state`,
    GET_DEVICES: `${BASE_NAME}/config/devices/get`,
    LAST_SEEN: `${BASE_NAME}/config/last_seen`,
    ELAPSED: `${BASE_NAME}/config/elapsed`,
    RESET: `${BASE_NAME}/config/reset`,
    TOUCHLINK_FACTORY_RESET: `${BASE_NAME}/config/touchlink/factory_reset`,
    OTA_UPDATE: `${BASE_NAME}/ota_update/+`,
    LOG_LEVEL: `${BASE_NAME}/config/log_level`,
    DEVICE_OPTIONS: `${BASE_NAME}/config/device_options`,
    REMOVE: `${BASE_NAME}/config/remove`,
    BAN: `${BASE_NAME}/config/ban`,
    WHITELIST: `${BASE_NAME}/config/whitelist`,
    RENAME: `${BASE_NAME}/config/rename`,
    RENAME_LAST: `${BASE_NAME}/config/rename_last`,
    REMOVE_GROUP: `${BASE_NAME}/config/remove_group`,
    NETWORK_MAP: `${BASE_NAME}/networkmap`,
    GRAPHVIZ: `${BASE_NAME}/graphviz`
}

export const ZIGBEE_LOG_TYPES = {
    DEVICE_CONNECTED: 'device_connected',
    PAIRING: 'pairing',
    DEVICE_BAN: 'device_ban',
    DEVICE_BAN_FAILED: 'device_ban_failed',
    DEVICE_ANNOUNCED: 'device_announced',
    DEVICE_REMOVED: 'device_removed',
    DEVICE_REMOVED_FAILED: 'device_removed_failed',
    DEVICE_FORCE_REMOVED: 'device_force_removed',
    DEVICE_FORCE_REMOVED_FAILED: 'device_force_removed_failed',
    DEVICE_BANNED: 'device_banned',
    DEVICE_WHITELISTED: 'device_whitelisted',
    DEVICE_RENAMED: 'device_renamed',
    GROUP_RENAMED: 'group_renamed',
    GROUP_ADDED: 'group_added',
    GROUP_REMOVED: 'group_removed',
    DEVICE_BIND: 'device_bind',
    DEVICE_UNBIND: 'device_unbind',
    DEVICE_GROUP_ADD: 'device_group_add',
    DEVICE_GROUP_ADD_FAILED: 'device_group_add_failed',
    DEVICE_GROUP_REMOVE: 'device_group_remove',
    DEVICE_GROUP_REMOVE_FAILED: 'device_group_remove_failed',
    DEVICE_GROUP_REMOVE_ALL: 'device_group_remove_all',
    DEVICE_GROUP_REMOVE_ALL_FAILED: 'device_group_remove_all_failed',
    DEVICES: 'devices',
    GROUPS: 'groups',
    ZIGBEE_PUBLISH_ERROR: 'zigbee_publish_error',
    OTA_UPDATE: 'ota_update',
    TOUCHLINK: 'touchlink'
}
