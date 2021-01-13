import {screen} from "@testing-library/dom";
import userEvent from "@testing-library/user-event";

import {HausComponentHarness, renderFeatureComponent} from "../../../../testing";
import {DiagnosticsHeaderComponent} from "./diagnostics-header.component";
import {DiagnosticsModule} from "../../diagnostics.module";

export class DiagnosticsHeaderHarness extends HausComponentHarness<DiagnosticsHeaderComponent> {
  get connectionIcon() {
    return screen.getByTestId('connection-icon');
  }

  get connectionStatus() {
    return screen.getByTestId('connection-status');
  }

  get startDiscoveryElement() {
    return screen.getByTestId('start-discovery-btn');
  }

  get stopDiscoveryElement() {
    return screen.getByTestId('stop-discovery-btn');
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

  static async render(props: Partial<DiagnosticsHeaderComponent>) {
    const result = await renderFeatureComponent(DiagnosticsHeaderComponent, {
      imports: [DiagnosticsModule],
      componentProperties: props
    });

    return new DiagnosticsHeaderHarness(result);
  }
}
