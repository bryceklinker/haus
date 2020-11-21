import {DiagnosticsMessageModel} from "../../models";
import {renderFeatureComponent, TestingEventEmitter} from "../../../../testing";
import {DiagnosticsMessagesComponent} from "./diagnostics-messages.component";
import {DiagnosticsModule} from "../../diagnostics.module";
import {ModelFactory} from "../../../../testing/model-factory";
import {EventEmitter} from "@angular/core";

describe('DiagnosticsMessagesComponent', () => {
  it('should show each message', async () => {
    const messages: Array<DiagnosticsMessageModel> = [
      ModelFactory.createMqttDiagnosticsMessage(),
      ModelFactory.createMqttDiagnosticsMessage(),
      ModelFactory.createMqttDiagnosticsMessage()
    ];

    const {queryAllByTestId} = await renderMessages(messages);

    expect(queryAllByTestId('diagnostic-message')).toHaveLength(3);
  })

  it('should show message topic', async () => {
    const messages: Array<DiagnosticsMessageModel> = [
      ModelFactory.createMqttDiagnosticsMessage({topic: 'one'})
    ];

    const {getByTestId} = await renderMessages(messages);

    expect(getByTestId('diagnostic-message')).toHaveTextContent('one');
  })

  it('should show message payload', async () => {
    const messages: Array<DiagnosticsMessageModel> = [
      ModelFactory.createMqttDiagnosticsMessage({payload: '123'})
    ];

    const {getByTestId} = await renderMessages(messages);

    expect(getByTestId('diagnostic-message')).toHaveTextContent('123');
  })

  it('should show message payload json', async () => {
    const messages: Array<DiagnosticsMessageModel> = [
      ModelFactory.createMqttDiagnosticsMessage({payload: {id: 45, text: 'jack'}})
    ];

    const {getByTestId} = await renderMessages(messages);

    expect(getByTestId('diagnostic-message')).toHaveTextContent('45')
    expect(getByTestId('diagnostic-message')).toHaveTextContent('jack')
  })

  it('should trigger replay message when message is replayed', async () => {
    const model = ModelFactory.createMqttDiagnosticsMessage();
    const replayMessageEmitter = new TestingEventEmitter<DiagnosticsMessageModel>();

    const {getByTestId, fireEvent} = await renderMessages([model], replayMessageEmitter);

    fireEvent.click(getByTestId('replay-message-btn'));

    expect(replayMessageEmitter.emit).toHaveBeenCalledWith(model);
  })

  it('should disable replay button when message is being replayed', async () => {
    const model = ModelFactory.createMqttDiagnosticsMessage({isReplaying: true});

    const {getByTestId} = await renderMessages([model]);

    expect(getByTestId('replay-message-btn')).toBeDisabled();
  })

  async function renderMessages(messages: Array<DiagnosticsMessageModel>, replayMessage?: EventEmitter<DiagnosticsMessageModel>) {
    return await renderFeatureComponent(DiagnosticsMessagesComponent, {
      imports: [DiagnosticsModule],
      componentProperties: {
        messages,
        replayMessage: replayMessage || new TestingEventEmitter<DiagnosticsMessageModel>()
      }
    })
  }
})
