import {TestingEventEmitter, ModelFactory} from "../../../../testing";
import {DiagnosticsMessagesComponent} from "./diagnostics-messages.component";
import {UiMqttDiagnosticsMessageModel} from "../../../shared/models";
import {DiagnosticsMessagesHarness} from "./diagnostics-messages.harness";

describe('DiagnosticsMessagesComponent', () => {
  it('should show each message', async () => {
    const messages: Array<UiMqttDiagnosticsMessageModel> = [
      ModelFactory.createMqttDiagnosticsMessage(),
      ModelFactory.createMqttDiagnosticsMessage(),
      ModelFactory.createMqttDiagnosticsMessage()
    ];

    const harness = await DiagnosticsMessagesHarness.render({messages});

    expect(harness.messages).toHaveLength(3);
  })

  it('should show message topic', async () => {
    const messages: Array<UiMqttDiagnosticsMessageModel> = [
      ModelFactory.createMqttDiagnosticsMessage({topic: 'one'})
    ];

    const harness = await DiagnosticsMessagesHarness.render({messages});

    expect(harness.firstMessage).toHaveTextContent('one');
  })

  it('should show message payload', async () => {
    const messages: Array<UiMqttDiagnosticsMessageModel> = [
      ModelFactory.createMqttDiagnosticsMessage({payload: '123'})
    ];

    const harness = await DiagnosticsMessagesHarness.render({messages});

    expect(harness.firstMessage).toHaveTextContent('123');
  })

  it('should show message payload json', async () => {
    const messages: Array<UiMqttDiagnosticsMessageModel> = [
      ModelFactory.createMqttDiagnosticsMessage({payload: {id: 45, text: 'jack'}})
    ];

    const harness = await DiagnosticsMessagesHarness.render({messages});

    expect(harness.firstMessage).toHaveTextContent('45')
    expect(harness.firstMessage).toHaveTextContent('jack')
  })

  it('should trigger replay message when message is replayed', async () => {
    const model = ModelFactory.createMqttDiagnosticsMessage();
    const emitter = new TestingEventEmitter<UiMqttDiagnosticsMessageModel>();

    const harness = await DiagnosticsMessagesHarness.render({messages: [model], replayMessage: emitter});
    await harness.replayMessage();

    expect(emitter.emit).toHaveBeenCalledWith(model);
  })

  it('should disable replay button when message is being replayed', async () => {
    const model = ModelFactory.createMqttDiagnosticsMessage({isReplaying: true});

    const harness = await DiagnosticsMessagesHarness.render({messages: [model]});

    expect(harness.replayMessageElement).toBeDisabled();
  })
})
