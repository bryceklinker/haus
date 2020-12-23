import {ComponentFixture, TestBed} from "@angular/core/testing";
import {screen} from '@testing-library/dom'

import {
  eventually,
  ModelFactory,
  renderFeatureComponent, TestingServer, TestingSignalrConnectionServiceFactory,
  TestingSignalrHubConnectionService
} from "../../../../testing";
import {DiagnosticsModule} from "../../diagnostics.module";
import {DiagnosticsRootComponent} from "./diagnostics-root.component";
import {KNOWN_HUB_NAMES, SignalrHubConnectionFactory} from "../../../shared/signalr";
import userEvent from "@testing-library/user-event";
import {HttpMethod} from "../../../shared/rest-api";

describe('DiagnosticsRootComponent', () => {
  let hubConnection: TestingSignalrHubConnectionService;
  let fixture: ComponentFixture<DiagnosticsRootComponent>;

  beforeEach(async() => {
    const result = await renderRoot();
    hubConnection = result.signalrConnectionFactory.getTestingHub(KNOWN_HUB_NAMES.diagnostics);
    fixture = result.fixture;
  })

  it('should start connection to diagnostics when rendered', () => {
    expect(hubConnection.start).toHaveBeenCalled();
  })

  it('should stop connection when destroyed', () => {
    fixture.destroy();

    expect(hubConnection.stop).toHaveBeenCalled();
  })

  it('should render diagnostic messages', async () => {
    hubConnection.triggerMqttMessage(ModelFactory.createMqttDiagnosticsMessage());
    fixture.detectChanges();

    await eventually(() => {
      expect(screen.queryAllByTestId('diagnostic-message')).toHaveLength(1);
    })
  })

  it('should show diagnostic connection status', async () => {
    hubConnection.triggerStart();
    fixture.detectChanges();

    await eventually(() => {
      expect(screen.queryAllByText('connected')).toHaveLength(1);
    })
  })

  it('should start discovery', async () => {
    userEvent.click(screen.getByTestId('start-discovery-btn'));

    await eventually(() => {
      expect(TestingServer.lastRequest.url).toContain('/api/devices/start-discovery');
      expect(TestingServer.lastRequest.method).toEqual(HttpMethod.POST);
    })
  })

  it('should stop discovery', async () => {
    userEvent.click(screen.getByTestId('start-discovery-btn'));

    await eventually(() => {
      fixture.detectChanges();
      userEvent.click(screen.getByTestId('stop-discovery-btn'));

      expect(TestingServer.lastRequest.url).toContain('/api/devices/stop-discovery');
      expect(TestingServer.lastRequest.method).toEqual(HttpMethod.POST);
    })
  })

  it('should sync discovery', async () => {
    userEvent.click(screen.getByTestId('sync-discovery-btn'));

    await eventually(() => {
      expect(TestingServer.lastRequest.url).toContain('/api/devices/sync-discovery');
      expect(TestingServer.lastRequest.method).toEqual(HttpMethod.POST);
    })
  })

  it('should replay message when message is replayed', async () => {
    const messageToReplay = ModelFactory.createMqttDiagnosticsMessage();
    TestingServer.setupPost('/api/diagnostics/replay', messageToReplay);
    hubConnection.triggerMqttMessage(messageToReplay);
    fixture.detectChanges();

    userEvent.click(screen.getByTestId('replay-message-btn'));

    await eventually(() => {
      fixture.detectChanges();
      expect(TestingServer.lastRequest.url).toContain('/api/diagnostics/replay');
      expect(TestingServer.lastRequest.method).toEqual(HttpMethod.POST);
      expect(TestingServer.lastRequest.body).toEqual(messageToReplay);
    })
  })

  function renderRoot() {
    return renderFeatureComponent(DiagnosticsRootComponent, {
      imports: [DiagnosticsModule]
    })
  }
})
