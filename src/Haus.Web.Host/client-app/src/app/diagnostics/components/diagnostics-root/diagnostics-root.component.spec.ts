import {
  eventually,
  ModelFactory,
} from '../../../../testing';
import {DiagnosticsRootComponent} from './diagnostics-root.component';
import {DiagnosticsActions} from '../../state';
import {DiscoveryActions} from '../../../shared/discovery';
import {DiagnosticsRootHarness} from './diagnostics-root.harness';

describe('DiagnosticsRootComponent', () => {
  test('should start connection to diagnostics when rendered', async () => {
    const harness = await DiagnosticsRootHarness.render();

    expect(harness.dispatchedActions).toContainEqual(DiagnosticsActions.start());
  });

  test('should stop connection when destroyed', async () => {
    const harness = await DiagnosticsRootHarness.render();

    harness.destroy();

    expect(harness.dispatchedActions).toContainEqual(DiagnosticsActions.stop());
  });

  test('should render diagnostic messages', async () => {
    const harness = await DiagnosticsRootHarness.render(DiagnosticsActions.messageReceived(ModelFactory.createMqttDiagnosticsMessage()));

    await eventually(() => {
      expect(harness.messages).toHaveLength(1);
    });
  });

  test('should show diagnostic connection status', async () => {
    const harness = await DiagnosticsRootHarness.render(DiagnosticsActions.connected());

    await eventually(() => {
      expect(harness.connectionStatus).toHaveTextContent('connected');
      expect(harness.connectionStatus).not.toHaveTextContent('disconnected');
    });
  });

  test('should start discovery', async () => {
    const harness = await DiagnosticsRootHarness.render();

    await harness.startDiscovery();

    expect(harness.dispatchedActions).toContainEqual(DiscoveryActions.startDiscovery.request());
  });

  test('should stop discovery', async () => {
    const harness = await DiagnosticsRootHarness.render(DiscoveryActions.startDiscovery.success());

    await harness.stopDiscovery();

    expect(harness.dispatchedActions).toContainEqual(DiscoveryActions.stopDiscovery.request());
  });

  test('should sync discovery', async () => {
    const harness = await DiagnosticsRootHarness.render();

    await harness.syncDiscovery();

    expect(harness.dispatchedActions).toContainEqual(DiscoveryActions.syncDiscovery.request());
  });

  test('should replay message when message is replayed', async () => {
    const messageToReplay = ModelFactory.createMqttDiagnosticsMessage();

    const harness = await DiagnosticsRootHarness.render(DiagnosticsActions.messageReceived(messageToReplay));

    await harness.replayMessage();

    expect(harness.dispatchedActions).toContainEqual(DiagnosticsActions.replayMessage.request(messageToReplay));
  });

  test('when filter changes then displayed messages are filtered', async () => {
    const harness = await DiagnosticsRootHarness.render(
      DiagnosticsActions.messageReceived(ModelFactory.createMqttDiagnosticsMessage({
        topic: 'three'
      })),
      DiagnosticsActions.messageReceived(ModelFactory.createMqttDiagnosticsMessage({
        topic: 'four'
      }))
    );

    harness.enterFilterTopic('four');

    await eventually(() => expect(harness.messages).toHaveLength(1));
  })
});
