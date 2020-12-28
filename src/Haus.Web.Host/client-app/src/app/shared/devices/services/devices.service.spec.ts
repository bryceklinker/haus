import {TestBed} from "@angular/core/testing";
import {ActivatedRoute} from "@angular/router";

import {DevicesService} from "./devices.service";
import {DeviceModel} from "../models";
import {SharedModule} from "../../shared.module";
import {
  createFeatureTestingService,
  eventually,
  ModelFactory,
  TestingServer,
  TestingActivatedRoute,
  setupGetAllDevices, setupDeviceTurnOff, setupDeviceTurnOn
} from "../../../../testing";
import {HttpMethod} from "../../rest-api";

describe('DevicesService', () => {
  let activatedRoute: TestingActivatedRoute;
  let service: DevicesService;
  let devices: DeviceModel[] | null;
  let selectedDevice: DeviceModel | null;

  beforeEach(() => {
    const result = createFeatureTestingService(DevicesService, {imports: [SharedModule]});

    service = result.service;
    activatedRoute = <TestingActivatedRoute>TestBed.inject(ActivatedRoute);

    devices = null;
    selectedDevice = null;
    service.devices$.subscribe(d => devices = d);
    service.selectedDevice$.subscribe(d => selectedDevice = d);
  })

  it('should have empty devices', async () => {
    await eventually(() => {
      expect(devices).toEqual([]);
    })
  })

  it('should notify when devices are loaded', async () => {
    const expected = [
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel()
    ];
    setupGetAllDevices(expected);

    service.getAll().subscribe();

    await eventually(() => {
      expect(devices).toContainEqual(expected[0]);
      expect(devices).toContainEqual(expected[1]);
      expect(devices).toContainEqual(expected[2]);
    })
  })

  it('should sort devices by name', async () => {
    const second = ModelFactory.createDeviceModel({name: 'B'});
    const third = ModelFactory.createDeviceModel({name: 'C'});
    const first = ModelFactory.createDeviceModel({name: 'A'});
    setupGetAllDevices([second, third, first]);

    service.getAll().subscribe();

    await eventually(() => {
      if (devices === null) throw new Error('Devices is still null');
      expect(devices[0]).toEqual(first);
      expect(devices[1]).toEqual(second);
      expect(devices[2]).toEqual(third);
    })
  })

  it('should turn off device using api', async () => {
    setupDeviceTurnOff(6);

    service.turnOff(6).subscribe();

    await eventually(() => {
      expect(TestingServer.lastRequest.url).toContain('/api/devices/6/turn-off');
      expect(TestingServer.lastRequest.method).toEqual(HttpMethod.POST);
    })
  })

  it('should turn on device using api', async () => {
    setupDeviceTurnOn(7);

    service.turnOn(7).subscribe();

    await eventually(() => {
      expect(TestingServer.lastRequest.url).toContain('/api/devices/7/turn-on');
      expect(TestingServer.lastRequest.method).toEqual(HttpMethod.POST);
    })
  })

  it('should have null selected device', async () => {
    setupGetAllDevices();
    
    service.getAll().subscribe();

    await eventually(() => {
      expect(selectedDevice).toBeNull();
    })
  })

  it('should notify that selected device changed when route changes', async () => {
    const device = ModelFactory.createDeviceModel();
    setupGetAllDevices([device]);

    service.getAll().subscribe();
    activatedRoute.triggerParamsChange({'deviceId': `${device.id}`});

    await eventually(() => {
      expect(selectedDevice).toEqual(device);
    })
  })
})
