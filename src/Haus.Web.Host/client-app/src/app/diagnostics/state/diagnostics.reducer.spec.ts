import {generateStateFromActions} from "../../../testing/app-state-generator";
import {diagnosticsReducer} from "./diagnostics.reducer";
import {DiagnosticsActions} from "./actions";
import {ModelFactory} from "../../../testing";

describe('diagnosticsReducer', () => {
  it('should add message when message is received', () => {
    const message = ModelFactory.createMqttDiagnosticsMessage();

    const state = generateStateFromActions(diagnosticsReducer, DiagnosticsActions.messageReceived(message));

    expect(state.entities[message.id]).toEqual(message);
  })

  it('should mark message as being replayed when message reply is requested', () => {
    const message = ModelFactory.createMqttDiagnosticsMessage();

    const state = generateStateFromActions(
      diagnosticsReducer,
      DiagnosticsActions.replayMessage.request(message)
    );

    expect(state.entities[message.id]?.isReplaying).toEqual(true);
  })

  it('should mark message as not replaying when message replay is successful', () => {
    const message = ModelFactory.createMqttDiagnosticsMessage();

    const state = generateStateFromActions(
      diagnosticsReducer,
      DiagnosticsActions.replayMessage.request(message),
      DiagnosticsActions.replayMessage.success(message)
    );

    expect(state.entities[message.id]?.isReplaying).toEqual(false);
  })

  it('should be connected', () => {
    const state = generateStateFromActions(diagnosticsReducer,
      DiagnosticsActions.connected());

    expect(state.isConnected).toEqual(true);
  })

  it('should be disconnected', () => {
    const state = generateStateFromActions(diagnosticsReducer,
      DiagnosticsActions.connected(),
      DiagnosticsActions.disconnected());

    expect(state.isConnected).toEqual(false);
  })
})
