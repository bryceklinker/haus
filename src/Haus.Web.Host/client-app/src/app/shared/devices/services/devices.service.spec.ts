import {TestBed} from "@angular/core/testing";
import {ActivatedRoute} from "@angular/router";

import {DevicesService} from "./devices.service";
import {DeviceModel} from "../models";
import {SharedModule} from "../../shared.module";
import {createTestingService, eventually, ModelFactory, TestingServer, TestingActivatedRoute} from "../../../../testing";
import {HttpMethod} from "../../rest-api";
import {HttpStatusCodes} from "../../rest-api/http-status-codes";

describe('DevicesService', () => {
  let activatedRoute: TestingActivatedRoute;
  let service: DevicesService;
  let devices: DeviceModel[] | null;
  let selectedDevice: DeviceModel | null;

  beforeEach(() => {
    const result = createTestingService(DevicesService, {imports: [SharedModule]});

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
    const expected = ModelFactory.createListResult(
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel()
    )
    TestingServer.setupGet('/api/devices', expected);

    service.getAll();

    await eventually(() => {
      expect(devices).toContainEqual(expected.items[0]);
      expect(devices).toContainEqual(expected.items[1]);
      expect(devices).toContainEqual(expected.items[2]);
    })
  })

  it('should sort devices by name', async () => {
    const second = ModelFactory.createDeviceModel({name: 'B'});
    const third = ModelFactory.createDeviceModel({name: 'C'});
    const first = ModelFactory.createDeviceModel({name: 'A'});
    TestingServer.setupGet('/api/devices', ModelFactory.createListResult(second, third, first));

    service.getAll();

    await eventually(() => {
      if (devices === null) throw new Error('Devices is still null');
      expect(devices[0]).toEqual(first);
      expect(devices[1]).toEqual(second);
      expect(devices[2]).toEqual(third);
    })
  })

  it('should turn off device using api', async () => {
    TestingServer.setupPost('/api/devices/6/turn-off', null, {status: HttpStatusCodes.NoContent});

    service.turnOff(6);

    await eventually(() => {
      expect(TestingServer.lastRequest.url).toContain('/api/devices/6/turn-off');
      expect(TestingServer.lastRequest.method).toEqual(HttpMethod.POST);
    })
  })

  it('should turn on device using api', async () => {
    TestingServer.setupPost('/api/devices/7/turn-on', null, {status: HttpStatusCodes.NoContent});

    service.turnOn(7);

    await eventually(() => {
      expect(TestingServer.lastRequest.url).toContain('/api/devices/7/turn-on');
      expect(TestingServer.lastRequest.method).toEqual(HttpMethod.POST);
    })
  })

  it('should have null selected device', async () => {
    service.getAll();

    await eventually(() => {
      expect(selectedDevice).toBeNull();
    })
  })

  it('should notify that selected device changed when route changes', async () => {
    const device = ModelFactory.createDeviceModel();
    TestingServer.setupGet('/api/devices', ModelFactory.createListResult(device));

    service.getAll();
    activatedRoute.triggerParamsChange({'deviceId': `${device.id}`});

    await eventually(() => {
      expect(selectedDevice).toEqual(device);
    })
  })
})
