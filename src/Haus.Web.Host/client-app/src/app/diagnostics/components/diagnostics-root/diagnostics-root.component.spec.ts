import {screen} from '@testing-library/dom'
import userEvent from "@testing-library/user-event";
import {Action} from "@ngrx/store";

import {
  eventually,
  ModelFactory,
  renderFeatureComponent,
} from "../../../../testing";
import {DiagnosticsModule} from "../../diagnostics.module";
import {DiagnosticsRootComponent} from "./diagnostics-root.component";
import {DiagnosticsActions} from "../../state";

describe('DiagnosticsRootComponent', () => {
  it('should start connection to diagnostics when rendered', async () => {
    const {store} = await renderRoot();

    expect(store.dispatchedActions).toContainEqual(DiagnosticsActions.start());
  })

  it('should stop connection when destroyed', async () => {
    const {fixture, store} = await renderRoot();

    fixture.destroy();

    expect(store.dispatchedActions).toContainEqual(DiagnosticsActions.stop());
  })

  it('should render diagnostic messages', async () => {
    await renderRoot(DiagnosticsActions.messageReceived(ModelFactory.createMqttDiagnosticsMessage()));

    await eventually(() => {
      expect(screen.queryAllByTestId('diagnostic-message')).toHaveLength(1);
    })
  })

  it('should show diagnostic connection status', async () => {
    await renderRoot(DiagnosticsActions.connected());

    await eventually(() => {
      expect(screen.queryAllByText('connected')).toHaveLength(1);
    })
  })

  it('should start discovery', async () => {
    const {store} = await renderRoot();

    userEvent.click(screen.getByTestId('start-discovery-btn'));

    expect(store.dispatchedActions).toContainEqual(DiagnosticsActions.startDiscovery.request());
  })

  it('should stop discovery', async () => {
    const {store} = await renderRoot(DiagnosticsActions.startDiscovery.success());

    userEvent.click(screen.getByTestId('stop-discovery-btn'));

    expect(store.dispatchedActions).toContainEqual(DiagnosticsActions.stopDiscovery.request());
  })

  it('should sync discovery', async () => {
    const {store} = await renderRoot();

    userEvent.click(screen.getByTestId('sync-discovery-btn'));

    expect(store.dispatchedActions).toContainEqual(DiagnosticsActions.syncDiscovery.request());
  })

  it('should replay message when message is replayed', async () => {
    const messageToReplay = ModelFactory.createMqttDiagnosticsMessage();

    const {store} = await renderRoot(DiagnosticsActions.messageReceived(messageToReplay));

    userEvent.click(screen.getByTestId('replay-message-btn'));

    expect(store.dispatchedActions).toContainEqual(DiagnosticsActions.replayMessage.request(messageToReplay));
  })

  function renderRoot(...actions: Array<Action>) {
    return renderFeatureComponent(DiagnosticsRootComponent, {
      imports: [DiagnosticsModule],
      actions: actions
    })
  }
})
