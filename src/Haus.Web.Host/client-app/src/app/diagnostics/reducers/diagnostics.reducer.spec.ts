import {diagnosticsReducer} from "./diagnostics.reducer";
import {initAction} from "../../../testing";
import {DiagnosticsActions} from "../actions";

describe('diagnosticsReducer', () => {
  it('should return initial state', () => {
    const state = diagnosticsReducer(undefined, initAction());
    expect(state.messages).toEqual([]);
  })

  it('should add message to state when mqtt message received', () => {
    let state = diagnosticsReducer(undefined, initAction());
    state = diagnosticsReducer(state, DiagnosticsActions.messageReceived({topic: 'hello', payload: 'my hero'}));

    expect(state.messages).toContainEqual({
      topic: 'hello',
      payload: 'my hero'
    })
  })
})
