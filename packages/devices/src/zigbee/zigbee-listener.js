import Zigbee2MqttController from 'zigbee2mqtt/lib/controller';
import logger from 'zigbee2mqtt/lib/util/logger';
import utils from 'zigbee2mqtt/lib/util/utils';
import settings from 'zigbee2mqtt/lib/util/settings';

export class ZigbeeListener extends Zigbee2MqttController {
    async start() {
        this.state.start();

        const info = await utils.getZigbee2mqttVersion();
        logger.info(`Starting Zigbee2MQTT version ${info.version} (commit #${info.commitHash})`);

        // Start zigbee
        try {
            await this.zigbee.start();
            await this.callExtensionMethod('onZigbeeStarted', []);
            this.zigbee.on('event', this.onZigbeeEvent.bind(this));
            this.zigbee.on('adapterDisconnected', this.onZigbeeAdapterDisconnected);
        } catch (error) {
            logger.error('Failed to start zigbee');
            logger.error('Exiting...');
            logger.error(error.stack);
            return;
        }

        // Log zigbee clients on startup
        const devices = this.zigbee.getClients();
        logger.info(`Currently ${devices.length} devices are joined:`);
        for (const device of devices) {
            const entity = this.zigbee.resolveEntity(device);
            const model = entity.definition ?
                `${entity.definition.model} - ${entity.definition.vendor} ${entity.definition.description}` :
                'Not supported';
            logger.info(`${entity.name} (${entity.device.ieeeAddr}): ${model} (${entity.device.type})`);
        }

        // Enable zigbee join.
        if (settings.get().permit_join) {
            logger.warn('`permit_join` set to  `true` in configuration.yaml.');
            logger.warn('Allowing new devices to join.');
            logger.warn('Set `permit_join` to `false` once you joined all devices.');
        }

        try {
            await this.zigbee.permitJoin(settings.get().permit_join);
        } catch (error) {
            logger.error(`Failed to set permit join to ${settings.get().permit_join}`);
        }

        // MQTT
        this.mqtt.on('message', this.onMQTTMessage.bind(this));
        await this.mqtt.connect();

        // Send all cached states.
        if (settings.get().advanced.cache_state_send_on_startup && settings.get().advanced.cache_state) {
            for (const device of this.zigbee.getClients()) {
                if (this.state.exists(device.ieeeAddr)) {
                    this.publishEntityState(device.ieeeAddr, this.state.get(device.ieeeAddr));
                }
            }
        }

        // Add devices which are in database but not in configuration to configuration
        for (const device of this.zigbee.getClients()) {
            if (!settings.getDevice(device.ieeeAddr)) {
                settings.addDevice(device.ieeeAddr);
            }
        }

        // Call extensions
        await this.callExtensionMethod('onMQTTConnected', []);
    }

    async stop() {
        // Call extensions
        await this.callExtensionMethod('stop', []);

        // Wrap-up
        this.state.stop();
        await this.mqtt.disconnect();

        try {
            await this.zigbee.stop();
        } catch (error) {
            logger.error('Failed to stop zigbee');
        }
    }
}
