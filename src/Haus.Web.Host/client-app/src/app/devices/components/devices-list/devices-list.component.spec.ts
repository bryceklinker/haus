import {renderFeatureComponent, ModelFactory, TestingEventEmitter} from "../../../../testing";
import {DevicesListComponent} from "./devices-list.component";
import {DevicesModule} from "../../devices.module";

describe('DevicesListComponent', () => {
  it('should show each device name', async () => {
    const devices = [
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel()
    ];

    const {queryAllByTestId} = await renderDevicesList({devices})

    expect(queryAllByTestId('device-item')).toHaveLength(3);
  })

  it('should notify when device is selected', async () => {
    const devices = [
      ModelFactory.createDeviceModel({id: 12})
    ];

    const emitter = new TestingEventEmitter<number>();
    const {getByTestId, fireEvent} = await renderDevicesList({devices, deviceSelected: emitter});
    fireEvent.click(getByTestId('device-item'));

    expect(emitter.emit).toHaveBeenCalledWith(12);
  })

  function renderDevicesList({devices = [], deviceSelected = new TestingEventEmitter<number>()}: Partial<DevicesListComponent> = {}) {
    return renderFeatureComponent(DevicesListComponent, {
      imports: [DevicesModule],
      componentProperties: {devices, deviceSelected}
    })
  }
})
