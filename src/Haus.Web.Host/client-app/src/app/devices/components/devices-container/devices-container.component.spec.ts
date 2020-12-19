import {Action} from "@ngrx/store";

import {renderFeatureComponent, ModelFactory, TestingActions} from "../../../../testing";
import {DevicesModule} from "../../devices.module";
import {DevicesContainerComponent} from "./devices-container.component";
import {ENTITY_NAMES} from "../../../entity-metadata";

describe('DevicesContainer', () => {
  it('should load devices when rendered', async () => {
    const {store} = await renderContainer();

    expect(store.actions).toContainEntityAction(TestingActions.createQueryAll(ENTITY_NAMES.Device));
  })

  it('should show devices list', async () => {
    const action = TestingActions.createQueryAllSuccess(ENTITY_NAMES.Device, [
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel()
    ])
    const {queryAllByTestId} = await renderContainer(action);

    expect(queryAllByTestId('device-item')).toHaveLength(3);
  })

  async function renderContainer(...actions: Action[]) {
    return await renderFeatureComponent(DevicesContainerComponent, {
      imports: [DevicesModule],
      actions
    });
  }
})
