import {diagnosticsReducer} from "./diagnostics.reducer";
import {runActionsThroughReducer} from "../../../testing";
import {DiagnosticsActions} from "../actions";
import {ModelFactory} from "../../../testing/model-factory";

describe('diagnosticsReducer', () => {
  it('should return initial state', () => {
    const state = runActionsThroughReducer(diagnosticsReducer);
    expect(state.messages).toEqual([]);
  })

  it('should add message to state when mqtt message received', () => {
    const model = ModelFactory.createMqttDiagnosticsMessage();
    let state = runActionsThroughReducer(diagnosticsReducer,
      DiagnosticsActions.messageReceived(model));

    expect(state.messages).toContainEqual(model);
  })

  it('should mark message as replaying when message replay is requested', () => {
    const model = ModelFactory.createMqttDiagnosticsMessage({id: '6'});
    const state = runActionsThroughReducer(diagnosticsReducer,
      DiagnosticsActions.messageReceived(model),
      DiagnosticsActions.replayMessageRequest(model));

    expect(state.messages).toContainEqual({
      ...model,
      isReplaying: true
    })
  })

  it('should update message when replay fails', () => {
    const error = new Error();
    const model = ModelFactory.createMqttDiagnosticsMessage({id: '6'});

    const state = runActionsThroughReducer(diagnosticsReducer,
      DiagnosticsActions.messageReceived(model),
      DiagnosticsActions.replayMessageRequest(model),
      DiagnosticsActions.replayMessageFailed(model, error));

    expect(state.messages).toContainEqual({
      ...model,
      isReplaying: false,
      replayError: error
    })
  })
})
