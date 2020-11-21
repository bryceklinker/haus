import {diagnosticsReducer, selectDiagnosticsMessages} from "./diagnostics.reducer";
import {createTestingState, runActionsThroughReducer} from "../../../testing";
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

  it('should select messages in most recent event first', () => {
    const first = ModelFactory.createMqttDiagnosticsMessage({timestamp: '2020-09-13'});
    const second = ModelFactory.createMqttDiagnosticsMessage({timestamp: '2020-09-12'});
    const third = ModelFactory.createMqttDiagnosticsMessage({timestamp: '2020-09-11'});

    const appState = createTestingState(
      DiagnosticsActions.messageReceived(third),
      DiagnosticsActions.messageReceived(first),
      DiagnosticsActions.messageReceived(second))

    const ordered = selectDiagnosticsMessages(appState)
    expect(ordered[0]).toEqual(first);
    expect(ordered[1]).toEqual(second);
    expect(ordered[2]).toEqual(third);
  })
})
