import {screen} from "@testing-library/dom";
import {eventually, ModelFactory, renderFeatureComponent,} from "../../../../testing";
import {DeviceDetailRootComponent} from "./device-detail-root.component";
import {DevicesModule} from "../../devices.module";
import {Action} from "@ngrx/store";
import {DevicesActions} from "../../state";
import {MatButtonHarness} from "@angular/material/button/testing";
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

  it('should notify to save device lighting constraints', async () => {
    const device = ModelFactory.createDeviceModel({deviceType: DeviceType.Light});
    const actions = DevicesActions.loadDevices.success(ModelFactory.createListResult(device));

    const {activatedRoute, detectChanges, matHarness, store, fixture} = await renderRoot(actions);
    activatedRoute.triggerParamsChange({deviceId: `${device.id}`});
    detectChanges();

    const save = await matHarness.getHarness(MatButtonHarness.with({selector: '[data-testid="save-constraints-btn"]'}));
    await save.click();

    expect(store.dispatchedActions).toContainEqual(DevicesActions.changeDeviceLightingConstraints.request({
      device: device,
      constraints: device.lighting.constraints
    }))
  })

  function renderRoot(...actions: Action[]) {
    return renderFeatureComponent(DeviceDetailRootComponent, {
      imports: [DevicesModule],
      actions: actions
    })
  }
})
