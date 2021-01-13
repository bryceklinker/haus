import {HausComponentHarness, renderFeatureComponent, TestingEventEmitter} from "../../../../testing";
import {DiagnosticsMessagesComponent} from "./diagnostics-messages.component";
import {DiagnosticsModule} from "../../diagnostics.module";
import {screen} from "@testing-library/dom";
import userEvent from "@testing-library/user-event";

export class DiagnosticsMessagesHarness extends HausComponentHarness<DiagnosticsMessagesComponent> {
  get messages() {
    return screen.queryAllByTestId('diagnostic-message')
  }

  get firstMessage() {
    return this.messages[0];
  }

  get replayMessageElement() {
     return screen.getByTestId('replay-message-btn');
  }

  async replayMessage() {
    userEvent.click(screen.getByTestId('replay-message-btn'));
    await this.whenRenderingDone();
  }

  static async render(props: Partial<DiagnosticsMessagesComponent>) {
    const result = await renderFeatureComponent(DiagnosticsMessagesComponent, {
      imports: [DiagnosticsModule],
      componentProperties: props
    });

    return new DiagnosticsMessagesHarness(result);
  }
}
