import {renderFeatureComponent, ModelFactory} from "../../../../testing";
import {DevicesListComponent} from "./devices-list.component";
import {DevicesModule} from "../../devices.module";

describe('DevicesListComponent', () => {
  it('should show each device name', async () => {
    const devices = [
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel()
    ];

    const {queryAllByTestId, container} = await renderDevicesList({devices})

    expect(queryAllByTestId('device-item')).toHaveLength(3);
    expect(container).toHaveTextContent(devices[0].name);
    expect(container).toHaveTextContent(devices[1].name);
    expect(container).toHaveTextContent(devices[2].name);
  })

  it('should navigate to device when device selected', async () => {
    const devices = [
      ModelFactory.createDeviceModel({id: 12})
    ];

    const {getByTestId, fireEvent, router} = await renderDevicesList({devices});
    spyOn(router, 'navigate').and.callThrough();
    fireEvent.click(getByTestId('device-item'));

    expect(router.navigate).toHaveBeenCalledWith(['devices', 12]);
  })

  function renderDevicesList({devices = []}: Partial<DevicesListComponent> = {}) {
    return renderFeatureComponent(DevicesListComponent, {
      imports: [DevicesModule],
      componentProperties: {devices}
    })
  }
})
