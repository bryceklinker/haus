import {EventEmitter} from "@angular/core";
import {screen} from "@testing-library/dom";
import userEvent from "@testing-library/user-event";

import {renderFeatureComponent, TestingEventEmitter, ModelFactory} from "../../../../testing";
import {DiagnosticsMessagesComponent} from "./diagnostics-messages.component";
import {DiagnosticsModule} from "../../diagnostics.module";
import {UiMqttDiagnosticsMessageModel} from "../../../shared/models";

describe('DiagnosticsMessagesComponent', () => {
  it('should show each message', async () => {
    const messages: Array<UiMqttDiagnosticsMessageModel> = [
      ModelFactory.createMqttDiagnosticsMessage(),
      ModelFactory.createMqttDiagnosticsMessage(),
      ModelFactory.createMqttDiagnosticsMessage()
    ];

    await renderMessages(messages);

    expect(screen.queryAllByTestId('diagnostic-message')).toHaveLength(3);
  })

  it('should show message topic', async () => {
    const messages: Array<UiMqttDiagnosticsMessageModel> = [
      ModelFactory.createMqttDiagnosticsMessage({topic: 'one'})
    ];

    await renderMessages(messages);

    expect(screen.getByTestId('diagnostic-message')).toHaveTextContent('one');
  })

  it('should show message payload', async () => {
    const messages: Array<UiMqttDiagnosticsMessageModel> = [
      ModelFactory.createMqttDiagnosticsMessage({payload: '123'})
    ];

    await renderMessages(messages);

    expect(screen.getByTestId('diagnostic-message')).toHaveTextContent('123');
  })

  it('should show message payload json', async () => {
    const messages: Array<UiMqttDiagnosticsMessageModel> = [
      ModelFactory.createMqttDiagnosticsMessage({payload: {id: 45, text: 'jack'}})
    ];

    await renderMessages(messages);

    expect(screen.getByTestId('diagnostic-message')).toHaveTextContent('45')
    expect(screen.getByTestId('diagnostic-message')).toHaveTextContent('jack')
  })

  it('should trigger replay message when message is replayed', async () => {
    const model = ModelFactory.createMqttDiagnosticsMessage();
    const replayMessageEmitter = new TestingEventEmitter<UiMqttDiagnosticsMessageModel>();

    await renderMessages([model], replayMessageEmitter);

    userEvent.click(screen.getByTestId('replay-message-btn'));

    expect(replayMessageEmitter.emit).toHaveBeenCalledWith(model);
  })

  it('should disable replay button when message is being replayed', async () => {
    const model = ModelFactory.createMqttDiagnosticsMessage({isReplaying: true});

    await renderMessages([model]);

    expect(screen.getByTestId('replay-message-btn')).toBeDisabled();
  })

  async function renderMessages(messages: Array<UiMqttDiagnosticsMessageModel>, replayMessage?: EventEmitter<UiMqttDiagnosticsMessageModel>) {
    return await renderFeatureComponent(DiagnosticsMessagesComponent, {
      imports: [DiagnosticsModule],
      componentProperties: {
        messages,
        replayMessage: replayMessage || new TestingEventEmitter<UiMqttDiagnosticsMessageModel>()
      }
    })
  }
})
