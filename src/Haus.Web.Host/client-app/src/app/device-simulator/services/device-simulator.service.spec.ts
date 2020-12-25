import {DeviceSimulatorService} from "./device-simulator.service";
import {
  createTestingService, eventually, ModelFactory, TestingServer,
  TestingSettingsService,
  TestingSignalrConnectionServiceFactory,
  TestingSignalrHubConnectionService
} from "../../../testing";
import {DeviceSimulatorModule} from "../device-simulator.module";
import {TestBed} from "@angular/core/testing";
import {SettingsService} from "../../shared/settings";
import {SignalrHubConnectionFactory} from "../../shared/signalr";
import {DeviceSimulatorStateModel} from "../models";
import {HttpStatusCodes} from "../../shared/rest-api/http-status-codes";
import {HttpMethod} from "../../shared/rest-api";

describe('DeviceSimulatorService', () => {
  let service: DeviceSimulatorService;
  let settingService: TestingSettingsService;
  let hub: TestingSignalrHubConnectionService;

  beforeEach(() => {
    TestingSettingsService.updateSettings({
      deviceSimulator: {
        url: 'https://localhost:5005',
        isEnabled: true
      }
    })

    const result = createTestingService(DeviceSimulatorService, {imports: [DeviceSimulatorModule]});
    settingService = TestBed.inject(SettingsService) as TestingSettingsService;
    hub = (TestBed.inject(SignalrHubConnectionFactory) as TestingSignalrConnectionServiceFactory).getTestingHub('https://localhost:5005/hubs/devices');

    service = result.service;
  })

  it('should connect to device simulator signalr', async () => {
    service.start().subscribe();

    await eventually(() => {
      expect(hub.start).toHaveBeenCalled();
    })
  })

  it('should stop signalr connection when stopped', async () => {
    service.stop().subscribe();

    expect(hub.stop).toHaveBeenCalled();
  })

  it('should notify when device state changes', async () => {
    const expected = ModelFactory.createDeviceSimulatorState();

    let state: DeviceSimulatorStateModel | null = null;
    service.state$.subscribe(s => state = s);
    service.start().subscribe();
    hub.triggerMessage('OnStateChange', expected);

    await eventually(() => {
      expect(state).toEqual(expected);
    })
  })

  it('should add device to simulator', async () => {
    TestingServer.setupPost('https://localhost:5005/api/devices', null, {status: HttpStatusCodes.OK});

    service.addDevice({deviceType: 'Light'}).subscribe();

    await eventually(() => {
      expect(TestingServer.lastRequest.url).toEqual('https://localhost:5005/api/devices');
      expect(TestingServer.lastRequest.method).toEqual(HttpMethod.POST);
      expect(TestingServer.lastRequest.body).toEqual({deviceType: 'Light'});
    })
  })

  it('should get device types from simulator', async () => {
    const expected = ModelFactory.createListResult('Light', 'LightSensor');
    TestingServer.setupGet('https://localhost:5005/api/deviceTypes', expected);

    let types: string[] = [];
    service.deviceTypes$.subscribe(t => types = t);

    service.getDeviceTypes().subscribe();
    await eventually(() => {
      expect(types).toEqual(expected.items);
    })
  })
})
