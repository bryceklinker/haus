import {screen} from "@testing-library/dom";
import {Action} from "@ngrx/store";
import {MatButtonHarness} from "@angular/material/button/testing";

import {eventually, ModelFactory, renderFeatureComponent,} from "../../../../testing";
import {DeviceDetailRootComponent} from "./device-detail-root.component";
import {DevicesModule} from "../../devices.module";
import {DevicesActions} from "../../state";
import {DeviceType} from "../../../shared/models";

describe('DeviceDetailRootComponent', () => {

  it('should show device detail when rendered', async () => {
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
