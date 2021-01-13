import {TestingEventEmitter} from "../../../../testing";
import {DiagnosticsHeaderComponent} from "./diagnostics-header.component";
import {DiagnosticsHeaderHarness} from "./diagnostics-header.harness";

describe('DiagnosticsHeaderComponent', () => {
  it('should show connected when connected', async () => {
    const harness = await DiagnosticsHeaderHarness.render({isConnected: true});

    expect(harness.connectionIcon).toHaveTextContent('sync');
    expect(harness.connectionStatus).toHaveTextContent('connected');
  })

  it('should show disconnected when not connected', async () => {
    const harness = await DiagnosticsHeaderHarness.render({isConnected: false});

    expect(harness.connectionIcon).toHaveTextContent('sync_disabled');
    expect(harness.connectionStatus).toHaveTextContent('disconnected');
  })

  it('should notify to start discovery when discovery is started', async () => {
    const emitter = new TestingEventEmitter();

    const harness = await DiagnosticsHeaderHarness.render({startDiscovery: emitter});
    await harness.startDiscovery();

    expect(emitter.emit).toHaveBeenCalled();
  })

  it('should disable start discovery when discovery is already allowed', async () => {
    const harness = await DiagnosticsHeaderHarness.render({allowDiscovery: true});

    expect(harness.startDiscoveryElement).toBeDisabled();
  })

  it('should notify to stop discovery when discovery is stopped', async () => {
    const emitter = new TestingEventEmitter();

    const harness = await DiagnosticsHeaderHarness.render({
      stopDiscovery: emitter,
      allowDiscovery: true
    });

    await harness.stopDiscovery();

    expect(emitter.emit).toHaveBeenCalled();
  })

  it('should disable stop discovery when discovery is disallowed', async () => {
    const harness = await DiagnosticsHeaderHarness.render({allowDiscovery: false});

    expect(harness.stopDiscoveryElement).toBeDisabled();
  })

  it('should notify to sync discovery when discovery is stopped', async () => {
    const emitter = new TestingEventEmitter();

    const harness = await DiagnosticsHeaderHarness.render({syncDiscovery: emitter});
    await harness.syncDiscovery();

    expect(emitter.emit).toHaveBeenCalled();
  })
})
