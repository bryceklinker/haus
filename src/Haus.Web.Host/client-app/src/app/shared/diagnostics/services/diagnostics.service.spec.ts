import {DiagnosticsService} from "./diagnostics.service";
import {
  createFeatureTestingService,
  eventually,
  ModelFactory,
  setupDevicesStartDiscovery, setupDevicesStopDiscovery, setupDevicesSyncDiscovery,
  setupDiagnosticsReplay,
  TestingServer,
  TestingSignalrConnectionServiceFactory,
  TestingSignalrHubConnectionService
} from "../../../../testing";
import {SharedModule} from "../../shared.module";
import {TestBed} from "@angular/core/testing";
import {HubStatus} from "../../models";
import {SignalrHubConnectionFactory} from "../../signalr";
import {DiagnosticsMessageModel} from "../models";
import {HttpMethod} from "../../rest-api";

describe('DiagnosticsService', () => {
  let status: HubStatus;
  let messages: DiagnosticsMessageModel[];
  let allowDiscovery: boolean;
  let service: DiagnosticsService;
  let testingConnection: TestingSignalrHubConnectionService;

  beforeEach(() => {
    const result = createFeatureTestingService(DiagnosticsService, {imports: [SharedModule]});
    const testingConnectionFactory  = TestBed.inject(SignalrHubConnectionFactory) as TestingSignalrConnectionServiceFactory;
    testingConnection = testingConnectionFactory.getTestingHub('diagnostics');

    service = result.service;
    service.status$.subscribe(s => status = s);
    service.messages$.subscribe(m => messages = m);
    service.allowDiscovery$.subscribe(a => allowDiscovery = a);
  })

  it('should be connecting when connection is started', async () => {
    service.start().subscribe();

    await eventually(() => {
      expect(status).toEqual(HubStatus.Connecting);
    })
  })

  it('should be connected when connection finishes starting', async () => {
    service.start().subscribe();
    testingConnection.triggerStart();

    await eventually(() => {
      expect(status).toEqual(HubStatus.Connected);
    })
  })

  it('should be disconnected when connection is stopped', async () => {
    service.start().subscribe();
    testingConnection.triggerStart();

    service.stop().subscribe();
    testingConnection.triggerStop();

    await eventually(() => {
      expect(status).toEqual(HubStatus.Disconnected);
    })
  })

  it('should subscribe to mqtt messages', async () => {
    const expected = ModelFactory.createMqttDiagnosticsMessage();

    service.start().subscribe();
    testingConnection.triggerMessage('OnMqttMessage', expected);

    await eventually(() => {
      expect(messages).toContainEqual(expected);
    })
  })

  it('should post message to rest api when message is replayed', async () => {
    setupDiagnosticsReplay();

    const model = ModelFactory.createMqttDiagnosticsMessage();
    service.replayMessage(model).subscribe();

    await eventually(() => {
      expect(TestingServer.lastRequest.url).toContain('/api/diagnostics/replay');
      expect(TestingServer.lastRequest.method).toEqual(HttpMethod.POST);
      expect(TestingServer.lastRequest.body).toEqual(model);
    })
  })

  it('should sort messages by timestamp in descending order', async () => {
    const third = ModelFactory.createMqttDiagnosticsMessage({timestamp: '2020-09-23T00:00:01'});
    const first = ModelFactory.createMqttDiagnosticsMessage({timestamp: '2020-09-23T00:00:03'});
    const second = ModelFactory.createMqttDiagnosticsMessage({timestamp: '2020-09-23T00:00:02'});

    service.start().subscribe();
    testingConnection.triggerMessage('OnMqttMessage', third);
    testingConnection.triggerMessage('OnMqttMessage', first);
    testingConnection.triggerMessage('OnMqttMessage', second);

    await eventually(() => {
      expect(messages[0].timestamp).toEqual('2020-09-23T00:00:03')
      expect(messages[1].timestamp).toEqual('2020-09-23T00:00:02')
      expect(messages[2].timestamp).toEqual('2020-09-23T00:00:01')
    })
  })

  it('should start discovery on rest api', async () => {
    setupDevicesStartDiscovery();

    service.startDiscovery().subscribe();

    await eventually(() => {
      expect(TestingServer.lastRequest.url).toContain('/api/devices/start-discovery');
      expect(TestingServer.lastRequest.method).toEqual(HttpMethod.POST);
    })
  })

  it('should stop discovery on rest api', async () => {
    setupDevicesStopDiscovery();

    service.stopDiscovery().subscribe();

    await eventually(() => {
      expect(TestingServer.lastRequest.url).toContain('/api/devices/stop-discovery');
      expect(TestingServer.lastRequest.method).toEqual('POST');
    })
  })

  it('should notify when discovery allowed changes', async () => {
    setupDevicesStopDiscovery();
    setupDevicesStartDiscovery();

    service.startDiscovery().subscribe();
    await eventually(() => {
      expect(allowDiscovery).toEqual(true);
    });

    service.stopDiscovery().subscribe();
    await eventually(() => {
      expect(allowDiscovery).toEqual(false);
    })
  })

  it('should sync discovery on rest api', async () => {
    setupDevicesSyncDiscovery();
    service.syncDiscovery().subscribe();

    await eventually(() => {
      expect(TestingServer.lastRequest.url).toContain('/api/devices/sync-discovery');
      expect(TestingServer.lastRequest.method).toEqual(HttpMethod.POST);
    })
  })

  afterEach(() => {
    service.ngOnDestroy();
  })
})