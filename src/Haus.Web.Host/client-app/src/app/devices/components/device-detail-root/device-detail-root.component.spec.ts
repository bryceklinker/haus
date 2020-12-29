import {screen} from "@testing-library/dom";
import {
  eventually,
  ModelFactory,
  renderFeatureComponent,
} from "../../../../testing";
import {DeviceDetailRootComponent} from "./device-detail-root.component";
import {DevicesModule} from "../../devices.module";
import {Action} from "@ngrx/store";
import {DevicesActions} from "../../state";

describe('DeviceDetailRootComponent', () => {

  it('should device detail when rendered', async () => {
    const device = ModelFactory.createDeviceModel();
    const actions = DevicesActions.loadDevices.success(ModelFactory.createListResult(device));

    const {activatedRoute, detectChanges} = await renderRoot(actions);
    activatedRoute.triggerParamsChange({deviceId: `${device.id}`});

    await eventually(() => {
      detectChanges();
      expect(screen.getByTestId('device-detail')).toHaveTextContent(device.name);
    })
  })

  function renderRoot(...actions: Action[]) {
    return renderFeatureComponent(DeviceDetailRootComponent, {
      imports: [DevicesModule],
      actions: actions
    })
  }
})
