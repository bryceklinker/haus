import {renderFeatureComponent, TestingEventEmitter} from "../../../../testing";
import {DiagnosticsHeaderComponent} from "./diagnostics-header.component";
import {DiagnosticsModule} from "../../diagnostics.module";
import {DevicesActions} from "../../../devices/actions";
import {EventEmitter} from "@angular/core";

describe('DiagnosticsHeaderComponent', () => {
  it('should show connected when connected', async () => {
    const {getByTestId} = await renderHeader({isConnected: true});

    expect(getByTestId('connection-icon')).toHaveTextContent('sync');
    expect(getByTestId('connection-status')).toHaveTextContent('connected');
  })

  it('should show disconnected when not connected', async () => {
    const {getByTestId} = await renderHeader({isConnected: false});

    expect(getByTestId('connection-icon')).toHaveTextContent('sync_disabled');
    expect(getByTestId('connection-status')).toHaveTextContent('disconnected');
  })

  it('should notify to start discovery when discovery is started', async () => {
    const emitter = new TestingEventEmitter();

    const {getByTestId, fireEvent} = await renderHeader({startDiscovery: emitter});

    fireEvent.click(getByTestId('start-discovery-btn'));

    expect(emitter.emit).toHaveBeenCalled();
  })

  it('should disable start discovery when discovery is already allowed', async () => {
    const {getByTestId} = await renderHeader({allowDiscovery: true});

    expect(getByTestId('start-discovery-btn')).toBeDisabled();
  })

  it('should notify to stop discovery when discovery is stopped', async () => {
    const emitter = new TestingEventEmitter();

    const {getByTestId, fireEvent} = await renderHeader({stopDiscovery: emitter});

    fireEvent.click(getByTestId('stop-discovery-btn'));

    expect(emitter.emit).toHaveBeenCalled();
  })

  it('should disable stop discovery when discovery is disallowed', async () => {
    const {getByTestId} = await renderHeader({allowDiscovery: false});

    expect(getByTestId('stop-discovery-btn')).toBeDisabled();
  })

  it('should notify to sync discovery when discovery is stopped', async () => {
    const emitter = new TestingEventEmitter();

    const {getByTestId, fireEvent} = await renderHeader({syncDiscovery: emitter});

    fireEvent.click(getByTestId('sync-discovery-btn'));

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
