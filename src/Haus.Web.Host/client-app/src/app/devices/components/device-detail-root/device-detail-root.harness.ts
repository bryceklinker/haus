import {Action} from "@ngrx/store";
import {screen} from "@testing-library/dom";

import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {DeviceDetailRootComponent} from "./device-detail-root.component";
import {DeviceModel} from "../../../shared/models";
import {DevicesModule} from "../../devices.module";
import {DeviceDetailHarness} from "../device-detail/device-detail.harness";

export class DeviceDetailRootHarness extends HausComponentHarness<DeviceDetailRootComponent> {
  private _deviceDetailHarness: DeviceDetailHarness;

  get deviceDetail() {
    return screen.getByLabelText('device detail');
  }

  private constructor(result: RenderComponentResult<DeviceDetailRootComponent>) {
    super(result);

    this._deviceDetailHarness = DeviceDetailHarness.fromResult(result);
  }

  async getLightTypes() {
    return await this._deviceDetailHarness.getLightTypesOptions();
  }

  saveDevice() {
    this._deviceDetailHarness.saveDevice();
  }

  saveConstraints() {
    this._deviceDetailHarness.saveConstraints();
  }

  static async render(device: DeviceModel, ...actions: Action[]) {
    const result = await DeviceDetailRootHarness.renderRoot(...actions);
    result.activatedRoute.triggerParamsChange({deviceId: `${device.id}`});

    result.detectChanges();
    await result.fixture.whenRenderingDone();

    return new DeviceDetailRootHarness(result);
  }

  private static renderRoot(...actions: Action[]) {
    return renderFeatureComponent(DeviceDetailRootComponent, {
      imports: [DevicesModule],
      actions: actions
    })
  }
}
