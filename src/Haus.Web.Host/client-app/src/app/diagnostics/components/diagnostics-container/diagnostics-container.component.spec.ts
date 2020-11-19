import {DiagnosticsContainerComponent} from "./diagnostics-container.component";
import {DiagnosticsModule} from "../../diagnostics.module";
import {createTestingState, renderFeatureComponent} from "../../../../testing";
import {signalrConnected} from "ngrx-signalr-core";
import {DIAGNOSTICS_HUB} from "../../effects/diagnostics-hub";
import {DiagnosticsActions} from "../../actions";

describe('DiagnosticsContainerComponent', () => {
  it('should show connection status', async () => {
    const {container} = await renderFeatureComponent(DiagnosticsContainerComponent, {
      imports: [DiagnosticsModule],
      state: createTestingState(signalrConnected(DIAGNOSTICS_HUB))
    });

    expect(container).toHaveTextContent(/[Cc]onnected/i)
  })

  it('should show messages', async () => {
    const {container} = await renderFeatureComponent(DiagnosticsContainerComponent, {
      imports: [DiagnosticsModule],
      state: createTestingState(DiagnosticsActions.messageReceived({topic: 'one', payload: 'idk'}))
    });

    expect(container).toHaveTextContent('one')
    expect(container).toHaveTextContent('idk')
  })
})
