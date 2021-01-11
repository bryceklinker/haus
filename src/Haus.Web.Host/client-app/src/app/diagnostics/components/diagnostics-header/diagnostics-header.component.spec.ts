import {screen} from "@testing-library/dom";
import userEvent from '@testing-library/user-event';
import {EventEmitter} from "@angular/core";

import {renderFeatureComponent, TestingEventEmitter} from "../../../../testing";
import {DiagnosticsHeaderComponent} from "./diagnostics-header.component";
import {DiagnosticsModule} from "../../diagnostics.module";

describe('DiagnosticsHeaderComponent', () => {
  it('should show connected when connected', async () => {
    await renderHeader({isConnected: true});

    expect(screen.getByTestId('connection-icon')).toHaveTextContent('sync');
    expect(screen.getByTestId('connection-status')).toHaveTextContent('connected');
  })

  it('should show disconnected when not connected', async () => {
    await renderHeader({isConnected: false});

    expect(screen.getByTestId('connection-icon')).toHaveTextContent('sync_disabled');
    expect(screen.getByTestId('connection-status')).toHaveTextContent('disconnected');
  })

  it('should notify to start discovery when discovery is started', async () => {
    const emitter = new TestingEventEmitter();

    await renderHeader({startDiscovery: emitter});

    userEvent.click(screen.getByTestId('start-discovery-btn'));

    expect(emitter.emit).toHaveBeenCalled();
  })

  it('should disable start discovery when discovery is already allowed', async () => {
    await renderHeader({allowDiscovery: true});

    expect(screen.getByTestId('start-discovery-btn')).toBeDisabled();
  })

  it('should notify to stop discovery when discovery is stopped', async () => {
    const emitter = new TestingEventEmitter();

    await renderHeader({
      stopDiscovery: emitter,
      allowDiscovery: true
    });

    userEvent.click(screen.getByTestId('stop-discovery-btn'));

    expect(emitter.emit).toHaveBeenCalled();
  })

  it('should disable stop discovery when discovery is disallowed', async () => {
    await renderHeader({allowDiscovery: false});

    expect(screen.getByTestId('stop-discovery-btn')).toBeDisabled();
  })

  it('should notify to sync discovery when discovery is stopped', async () => {
    const emitter = new TestingEventEmitter();

    await renderHeader({syncDiscovery: emitter});

    userEvent.click(screen.getByTestId('sync-discovery-btn'));

    expect(emitter.emit).toHaveBeenCalled();
  })

  async function renderHeader({
                                isConnected = false,
                                startDiscovery = new EventEmitter(),
                                stopDiscovery = new EventEmitter(),
                                syncDiscovery = new EventEmitter(),
                                allowDiscovery = false
                              } = {}) {
    return await renderFeatureComponent(DiagnosticsHeaderComponent, {
      imports: [DiagnosticsModule],
      componentProperties: {
        isConnected,
        allowDiscovery,
        startDiscovery,
        stopDiscovery,
        syncDiscovery
      }
    })
  }
})
