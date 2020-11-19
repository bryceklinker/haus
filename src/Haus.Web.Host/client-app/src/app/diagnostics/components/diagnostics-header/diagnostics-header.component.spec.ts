import {renderFeatureComponent} from "../../../../testing";
import {DiagnosticsHeaderComponent} from "./diagnostics-header.component";
import {DiagnosticsModule} from "../../diagnostics.module";

describe('DiagnosticsHeaderComponent', () => {
  it('should show connected when connected', async () => {
    const {getByTestId} = await renderHeader(true);

    expect(getByTestId('connection-icon')).toHaveTextContent('sync');
    expect(getByTestId('connection-status')).toHaveTextContent('connected');
  })

  it('should show disconnected when not connected', async () => {
    const {getByTestId} = await renderHeader(false);

    expect(getByTestId('connection-icon')).toHaveTextContent('sync_disabled');
    expect(getByTestId('connection-status')).toHaveTextContent('disconnected');
  })

  async function renderHeader(isConnected: boolean) {
    return await renderFeatureComponent(DiagnosticsHeaderComponent, {
      imports: [DiagnosticsModule],
      componentProperties: { isConnected }
    })
  }
})
