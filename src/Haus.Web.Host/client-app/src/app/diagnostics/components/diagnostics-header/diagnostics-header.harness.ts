import {screen} from "@testing-library/dom";

import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {DiagnosticsHeaderComponent} from "./diagnostics-header.component";
import {DiagnosticsModule} from "../../diagnostics.module";

export class DiagnosticsHeaderHarness extends HausComponentHarness<DiagnosticsHeaderComponent> {
  get connectionIcon() {
    return screen.getByLabelText('connection icon');
  }

  get connectionStatus() {
    return screen.getByLabelText('connection status');
  }

  get startDiscoveryElement() {
    return screen.getByRole('button', {name: 'start discovery'});
  }

  get stopDiscoveryElement() {
    return screen.getByRole('button', {name: 'stop discovery'});
  }

  async startDiscovery() {
    await this.clickButtonByLabel('start discovery');
  }

  async stopDiscovery() {
    await this.clickButtonByLabel('stop discovery');
  }

  async syncDiscovery() {
    await this.clickButtonByLabel('sync discovery');
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
