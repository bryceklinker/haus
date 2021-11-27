import {screen} from "@testing-library/dom";

import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {DevicesListComponent} from "./devices-list.component";
import {DevicesModule} from "../../devices.module";

export class DevicesListHarness extends HausComponentHarness<DevicesListComponent>{
  get deviceItems() {
    return screen.queryAllByLabelText('device item');
  }

  static fromResult(result: RenderComponentResult<any>) {
    return new DevicesListHarness(result);
  }

  static async render(props: Partial<DevicesListComponent>) {
    const result = await renderFeatureComponent(DevicesListComponent, {
      imports: [DevicesModule],
      componentProperties: props
    });

    return new DevicesListHarness(result);
  }
}
