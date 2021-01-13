import {HausComponentHarness, renderFeatureComponent} from "../../../../testing";
import {DiagnosticsRootComponent} from "./diagnostics-root.component";
import {DiagnosticsModule} from "../../diagnostics.module";
import {Action} from "@ngrx/store";
import {screen} from "@testing-library/dom";
import userEvent from "@testing-library/user-event";

export class DiagnosticsRootHarness extends HausComponentHarness<DiagnosticsRootComponent> {
  get messages() {
    return screen.queryAllByTestId('diagnostic-message');
  }

  get connectionStatus() {
    return screen.queryByTestId('connection-status');
  }

  async startDiscovery() {
    userEvent.click(screen.getByTestId('start-discovery-btn'));
    await this.whenRenderingDone();
  }

  async stopDiscovery() {
    userEvent.click(screen.getByTestId('stop-discovery-btn'));
    await this.whenRenderingDone();
  }

  async syncDiscovery() {
    userEvent.click(screen.getByTestId('sync-discovery-btn'));
    await this.whenRenderingDone();
  }

  async replayMessage() {
    userEvent.click(screen.getByTestId('replay-message-btn'));
    await this.whenRenderingDone();
  }

  static async render(...actions: Action[]) {
    const result = await renderFeatureComponent(DiagnosticsRootComponent, {
      imports: [DiagnosticsModule],
      actions: actions
    });

    return new DiagnosticsRootHarness(result);
  }
}
