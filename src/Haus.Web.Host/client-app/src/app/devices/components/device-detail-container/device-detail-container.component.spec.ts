import {Action} from "@ngrx/store";
import {createAppState, ModelFactory, renderFeatureComponent, TestingActions} from "../../../../testing";
import {DeviceDetailContainerComponent} from "./device-detail-container.component";
import {DevicesModule} from "../../devices.module";
import {DevicesActions} from "../../actions";

describe('DeviceDetailContainerComponent', () => {
  it('should show device currently selected', async () => {
    const devicesResult = ModelFactory.createListResult(
      ModelFactory.createDeviceModel({id: 99}),
      ModelFactory.createDeviceModel({id: 54, name: 'bobo'}),
      ModelFactory.createDeviceModel({id: 33})
    )
    const {container} = await renderDetailContainer(
      TestingActions.setRouterState({url: '/devices/54', params: {deviceId: '54'}}),
      DevicesActions.load.success(devicesResult)
    );

    expect(container).toHaveTextContent('bobo');
  })

  function renderDetailContainer(...actions: Action[]) {
    return renderFeatureComponent(DeviceDetailContainerComponent, {
      imports: [DevicesModule],
      state: createAppState(...actions),
      routes: [
        {path: 'devices/:deviceId', component: DeviceDetailContainerComponent}
      ]
    })
  }
})
