import {DiagnosticsContainerComponent} from "./diagnostics-container.component";
import {DiagnosticsModule} from "../../diagnostics.module";
import {createFeatureComponentFactory} from "../../../../testing/create-feature-component-factory";
import {signalrConnected} from "ngrx-signalr-core";
import {DIAGNOSTICS_HUB} from "../../effects/diagnostics-hub";

describe('DiagnosticsContainerComponent', () => {
  const componentFactory = createFeatureComponentFactory(DiagnosticsContainerComponent, DiagnosticsModule);

  it('should show connection status', () => {
    const spectator = componentFactory({
      actions: [signalrConnected(DIAGNOSTICS_HUB)]
    });

    expect(spectator.query('connection-status')).toContainText(/[Cc]onnected/i.exec);
  })
})
