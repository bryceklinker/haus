import {DiagnosticsContainerComponent} from "./diagnostics-container.component";
import {DiagnosticsModule} from "../../diagnostics.module";
import {renderFeatureComponent} from "../../../../testing/render-component";

describe('DiagnosticsContainerComponent', () => {
  it('should show connection status', async () => {
    const {} = await renderFeatureComponent(DiagnosticsContainerComponent, {
      imports: [DiagnosticsModule]
    });
  })
})
