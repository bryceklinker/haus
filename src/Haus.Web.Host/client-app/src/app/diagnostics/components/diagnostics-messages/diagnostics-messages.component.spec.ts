import {MqttDiagnosticsMessageModel} from "../../models/mqtt-diagnostics-message.model";
import {renderFeatureComponent} from "../../../../testing";
import {DiagnosticsMessagesComponent} from "./diagnostics-messages.component";
import {DiagnosticsModule} from "../../diagnostics.module";

describe('DiagnosticsMessagesComponent', () => {
  it('should show each message', async () => {
    const messages: Array<MqttDiagnosticsMessageModel> = [
      {topic: 'one', payload: '123'},
      {topic: 'one', payload: '123'},
      {topic: 'one', payload: '123'}
    ];

    const {queryAllByTestId} = await renderMessages(messages);

    expect(queryAllByTestId('diagnostic-message')).toHaveLength(3);
  })

  it('should show message topic', async () => {
    const messages: Array<MqttDiagnosticsMessageModel> = [
      {topic: 'one', payload: '123'}
    ];

    const {getByTestId} = await renderMessages(messages);

    expect(getByTestId('diagnostic-message')).toHaveTextContent('one');
  })

  it('should show message payload', async () => {
    const messages: Array<MqttDiagnosticsMessageModel> = [
      {topic: 'one', payload: '123'}
    ];

    const {getByTestId} = await renderMessages(messages);

    expect(getByTestId('diagnostic-message')).toHaveTextContent('123');
  })

  it('should show message payload json', async () => {
    const messages: Array<MqttDiagnosticsMessageModel> = [
      {topic: 'one', payload: {id: 45, text: 'jack'}}
    ];

    const {getByTestId} = await renderMessages(messages);

    expect(getByTestId('diagnostic-message')).toHaveTextContent('45')
    expect(getByTestId('diagnostic-message')).toHaveTextContent('jack')
  })

  async function renderMessages(messages: Array<MqttDiagnosticsMessageModel>) {
    return await renderFeatureComponent(DiagnosticsMessagesComponent, {
      imports: [DiagnosticsModule],
      componentProperties: {messages}
    })
  }
})
