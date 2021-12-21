import {TestingEventEmitter, ModelFactory} from '../../../../testing';
import {DiagnosticsMessagesComponent} from './diagnostics-messages.component';
import {UiMqttDiagnosticsMessageModel} from '../../../shared/models';
import {DiagnosticsMessagesHarness} from './diagnostics-messages.harness';
import { DiagnosticsFilterParams } from '../../models';

describe('DiagnosticsMessagesComponent', () => {
  test('should show each message', async () => {
    const messages: Array<UiMqttDiagnosticsMessageModel> = [
      ModelFactory.createMqttDiagnosticsMessage(),
      ModelFactory.createMqttDiagnosticsMessage(),
      ModelFactory.createMqttDiagnosticsMessage()
    ];

    const harness = await DiagnosticsMessagesHarness.render({messages});

    expect(harness.messages).toHaveLength(3);
  });

  test('should show message topic', async () => {
    const messages: Array<UiMqttDiagnosticsMessageModel> = [
      ModelFactory.createMqttDiagnosticsMessage({topic: 'one'})
    ];

    const harness = await DiagnosticsMessagesHarness.render({messages});

    expect(harness.firstMessage).toHaveTextContent('one');
  });

  test('should show message payload', async () => {
    const messages: Array<UiMqttDiagnosticsMessageModel> = [
      ModelFactory.createMqttDiagnosticsMessage({payload: '123'})
    ];

    const harness = await DiagnosticsMessagesHarness.render({messages});

    expect(harness.firstMessage).toHaveTextContent('123');
  });

  test('should show message payload json', async () => {
    const messages: Array<UiMqttDiagnosticsMessageModel> = [
      ModelFactory.createMqttDiagnosticsMessage({payload: {id: 45, text: 'jack'}})
    ];

    const harness = await DiagnosticsMessagesHarness.render({messages});

    expect(harness.firstMessage).toHaveTextContent('45');
    expect(harness.firstMessage).toHaveTextContent('jack');
  });

  test('should trigger replay message when message is replayed', async () => {
    const model = ModelFactory.createMqttDiagnosticsMessage();
    const emitter = new TestingEventEmitter<UiMqttDiagnosticsMessageModel>();

    const harness = await DiagnosticsMessagesHarness.render({messages: [model], replayMessage: emitter});
    await harness.replayMessage();

    expect(emitter.emit).toHaveBeenCalledWith(model);
  });

  test('should disable replay button when message is being replayed', async () => {
    const model = ModelFactory.createMqttDiagnosticsMessage({isReplaying: true});

    const harness = await DiagnosticsMessagesHarness.render({messages: [model]});

    expect(harness.replayMessageElement).toBeDisabled();
  });

  test('when messages and params are present then shows messages that match filter params', async () => {
    const messages = [
      ModelFactory.createMqttDiagnosticsMessage({topic: 'one'}),
      ModelFactory.createMqttDiagnosticsMessage({topic: 'two'}),
      ModelFactory.createMqttDiagnosticsMessage({topic: 'three'}),
    ];
    const filterParams: DiagnosticsFilterParams = {topic: 'o'}

    const harness = await DiagnosticsMessagesHarness.render({messages, filterParams});

    expect(harness.messages).toHaveLength(2);
    expect(harness.container).toHaveTextContent('one');
    expect(harness.container).toHaveTextContent('two');
  })
});
