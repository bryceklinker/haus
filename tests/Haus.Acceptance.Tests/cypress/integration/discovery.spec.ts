import {v4 as uuid} from 'uuid';
import {MQTT_CLIENT, Zigbee2MqttMessageFactory} from "../support";
import {DeviceDiscoveryPage} from "../support/pages";

describe('Discovery', () => {
    it('should show newly discovered devices', () => {
        cy.login();
        
        DeviceDiscoveryPage.navigate();
        const deviceId = uuid();
        MQTT_CLIENT.publishZigbee2MqttMessage(Zigbee2MqttMessageFactory.interviewSuccessful(deviceId));

        DeviceDiscoveryPage.findUnassignedDevices().should('have.length', 1);
    })
})