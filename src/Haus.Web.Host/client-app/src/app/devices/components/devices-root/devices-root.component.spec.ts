import {screen} from "@testing-library/dom";
import {eventually, ModelFactory, renderFeatureComponent} from "../../../../testing";
import {DevicesRootComponent} from "./devices-root.component";
import {DevicesModule} from "../../devices.module";
import {DevicesActions} from "../../state";
import {Action} from "@ngrx/store";

describe('DevicesRootComponent', () => {
  it('should get all devices when rendered', async () => {
    const { store } = await renderRoot();

    expect(store.dispatchedActions).toContainEqual(DevicesActions.loadDevices.request());
  })

  it('should show all devices', async () => {
    const devices = [
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel()
    ];

    await renderRoot(
      DevicesActions.loadDevices.success(ModelFactory.createListResult(...devices))
    );

    expect(screen.queryAllByTestId('device-item')).toHaveLength(3);
  })

  function renderRoot(...actions: Array<Action>) {
    return renderFeatureComponent(DevicesRootComponent, {
      imports: [DevicesModule],
      actions: actions
    })
  }
})
