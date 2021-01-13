import {Action} from "@ngrx/store";
import {screen} from "@testing-library/dom";

import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {DiagnosticsRootComponent} from "./diagnostics-root.component";
import {DiagnosticsModule} from "../../diagnostics.module";
import {DiagnosticsHeaderHarness} from "../diagnostics-header/diagnostics-header.harness";
import {DiagnosticsMessagesHarness} from "../diagnostics-messages/diagnostics-messages.harness";

export class DiagnosticsRootHarness extends HausComponentHarness<DiagnosticsRootComponent> {
  private _diagnosticsHeaderHarness: DiagnosticsHeaderHarness;
  private _diagnosticsMessagesHarness: DiagnosticsMessagesHarness;

  get messages() {
    return screen.queryAllByTestId('diagnostic-message');
  }

  get connectionStatus() {
    return screen.queryByTestId('connection-status');
  }

  private constructor(result: RenderComponentResult<DiagnosticsRootComponent>) {
    super(result);

    this._diagnosticsHeaderHarness = DiagnosticsHeaderHarness.fromResult(result);
    this._diagnosticsMessagesHarness = DiagnosticsMessagesHarness.fromResult(result);
  }

  async startDiscovery() {
    await this._diagnosticsHeaderHarness.startDiscovery();
  }

  async stopDiscovery() {
    await this._diagnosticsHeaderHarness.stopDiscovery();
  }

  async syncDiscovery() {
    await this._diagnosticsHeaderHarness.syncDiscovery();
  }

  async replayMessage() {
    await this._diagnosticsMessagesHarness.replayMessage();
  }

  static async render(...actions: Action[]) {
    const result = await renderFeatureComponent(DiagnosticsRootComponent, {
      imports: [DiagnosticsModule],
      actions: actions
    });

    return new DiagnosticsRootHarness(result);
  }
}
