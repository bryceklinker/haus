import {Action} from "@ngrx/store";

import {renderFeatureComponent, ModelFactory} from "../../../../testing";
import {DevicesModule} from "../../devices.module";
import {DevicesContainerComponent} from "./devices-container.component";
import {DevicesActions} from "../../actions";

describe('DevicesContainer', () => {
  it('should load devices when rendered', async () => {
    const {store} = await renderContainer();

    expect(store.actions).toContainEqual(DevicesActions.load.request());
  })

  it('should show devices list', async () => {
    const result = ModelFactory.createListResult(
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel()
    )
    const {queryAllByTestId} = await renderContainer(DevicesActions.load.success(result));

    expect(queryAllByTestId('device-item')).toHaveLength(3);
  })

  async function renderContainer(...actions: Action[]) {
    return await renderFeatureComponent(DevicesContainerComponent, {
      imports: [DevicesModule],
      actions: [...actions]
    });
  }
})
