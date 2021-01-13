import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {DiagnosticsMessagesComponent} from "./diagnostics-messages.component";
import {DiagnosticsModule} from "../../diagnostics.module";
import {screen} from "@testing-library/dom";

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
    await this.clickButtonByTestId('replay-message-btn');
  }

  static fromResult(result: RenderComponentResult<any>) {
    return new DiagnosticsMessagesHarness(result);
  }

  static async render(props: Partial<DiagnosticsMessagesComponent>) {
    const result = await renderFeatureComponent(DiagnosticsMessagesComponent, {
      imports: [DiagnosticsModule],
      componentProperties: props
    });

    return new DiagnosticsMessagesHarness(result);
  }
}
