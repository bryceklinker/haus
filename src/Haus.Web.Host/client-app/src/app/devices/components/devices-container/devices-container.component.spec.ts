import {renderFeatureComponent} from "../../../../testing";
import {DevicesModule} from "../../devices.module";
import {DevicesContainerComponent} from "./devices-container.component";
import {DevicesActions} from "../../actions";

describe('DevicesContainer', () => {
  it('should load devices when rendered', async () => {
    const {store} = await renderContainer();

    expect(store.actions).toContainEqual(DevicesActions.load.request());
  })

  async function renderContainer() {
    return await renderFeatureComponent(DevicesContainerComponent, {imports: [DevicesModule]});
  }
})
