import {screen} from "@testing-library/dom";

import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
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
    await this.clickButtonByTestId('start-discovery-btn');
  }

  async stopDiscovery() {
    await this.clickButtonByTestId('stop-discovery-btn');
  }

  async syncDiscovery() {
    await this.clickButtonByTestId('sync-discovery-btn');
  }

  static fromResult(result: RenderComponentResult<any>) {
    return new DiagnosticsHeaderHarness(result);
  }

  static async render(props: Partial<DiagnosticsHeaderComponent>) {
    const result = await renderFeatureComponent(DiagnosticsHeaderComponent, {
      imports: [DiagnosticsModule],
      componentProperties: props
    });

    return new DiagnosticsHeaderHarness(result);
  }
}
