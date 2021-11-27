import {v4 as uuid} from 'uuid';
import {Zigbee2MqttMessageFactory} from "../support";
import {DeviceDiscoveryPage} from "../support/pages";

describe('Discovery', () => {
    it('should show newly discovered devices', () => {
        cy.login();
        
        DeviceDiscoveryPage.navigate();
        const deviceId = uuid();
        cy.publishZigbeeMessage(Zigbee2MqttMessageFactory.createLightDiscoveredMessage(deviceId))

        DeviceDiscoveryPage.findUnassignedDevices().should('contain.text', deviceId);
    })
})