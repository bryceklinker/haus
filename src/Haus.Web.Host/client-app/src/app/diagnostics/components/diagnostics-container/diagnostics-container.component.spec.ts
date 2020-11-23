import {DiagnosticsContainerComponent} from "./diagnostics-container.component";
import {DiagnosticsModule} from "../../diagnostics.module";
import {createTestingState, renderFeatureComponent} from "../../../../testing";
import {signalrConnected} from "ngrx-signalr-core";
import {DIAGNOSTICS_HUB} from "../../effects/diagnostics-hub";
import {DiagnosticsActions} from "../../actions";
import {ModelFactory} from "../../../../testing/model-factory";
import {Action} from "@ngrx/store";

describe('DiagnosticsContainerComponent', () => {
  it('should dispatch initialize diagnostics hub', async () => {
    const {store} = await renderContainer();

    expect(store.actions).toContainEqual(DiagnosticsActions.initHub());
  })

  it('should show connection status', async () => {
    const {container} = await renderContainer(signalrConnected(DIAGNOSTICS_HUB));

    expect(container).toHaveTextContent(/[Cc]onnected/i)
  })

  it('should show messages', async () => {
    const model = ModelFactory.createMqttDiagnosticsMessage({payload: 'idk'});
    const {container} = await renderContainer(DiagnosticsActions.messageReceived(model));

    expect(container).toHaveTextContent(model.topic);
    expect(container).toHaveTextContent(model.payload);
  })

  it('should dispatch replay message request', async () => {
    const model = ModelFactory.createMqttDiagnosticsMessage();
    const {getByTestId, fireEvent, store} = await renderContainer(DiagnosticsActions.messageReceived(model));

    fireEvent.click(getByTestId('replay-message-btn'));

    expect(store.actions).toContainEqual(DiagnosticsActions.replay.request(model));
  })

  async function renderContainer(...actions: Array<Action>) {
    return await renderFeatureComponent(DiagnosticsContainerComponent, {
      imports: [DiagnosticsModule],
      state: createTestingState(...actions)
    })
  }
})
