import {v4 as uuid} from 'uuid';
import {MQTT_CLIENT, Zigbee2MqttMessageFactory} from '../support';
import {DeviceDiscoveryPage, RoomsPage} from '../support/pages';

describe('Assign Devices to Room', () => {
    let lightId: string, sensorId: string, roomName: string;

    beforeEach(() => {
        lightId = uuid();
        sensorId = uuid();
        roomName = uuid();
    });

    it('should allow you to add devices to a room', () => {
        cy.login();
        
        cy.publishZigbeeMessage(Zigbee2MqttMessageFactory.createLightDiscoveredMessage(lightId));
        cy.publishZigbeeMessage(Zigbee2MqttMessageFactory.createMotionSensorDiscoveredMessage(sensorId));

        RoomsPage.navigate();
        RoomsPage.addRoom(roomName);
        RoomsPage.selectRoom(roomName);
        RoomsPage.assignDevicesToRoom(lightId, sensorId);

        RoomsPage.findDevices().should('have.length', 2);
    });
});