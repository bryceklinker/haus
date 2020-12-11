import {Action} from "@ngrx/store";

import {createTestingState, renderFeatureComponent, ModelFactory} from "../../../../testing";
import {DevicesModule} from "../../devices.module";
import {DevicesContainerComponent} from "./devices-container.component";
import {DevicesActions} from "../../actions";
import {RouterActions} from "../../../shared/routing/actions";

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

  it('should notify to navigate', async () => {
    const result = ModelFactory.createListResult(
      ModelFactory.createDeviceModel({id: 65})
    );

    const {getByTestId, fireEvent, store} = await renderContainer(DevicesActions.load.success(result));

    fireEvent.click(getByTestId('device-item'));

    expect(store.actions).toContainEqual(RouterActions.navigate('/devices/65'));
  })

  async function renderContainer(...actions: Action[]) {

    return await renderFeatureComponent(DevicesContainerComponent, {
      imports: [DevicesModule],
      state: createTestingState(...actions)
    });
  }
})
