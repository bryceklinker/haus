import {screen} from "@testing-library/dom";
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

    const {container} = await renderDevicesList({devices})

    expect(screen.queryAllByTestId('device-item')).toHaveLength(3);
    expect(container).toHaveTextContent(devices[0].name);
    expect(container).toHaveTextContent(devices[1].name);
    expect(container).toHaveTextContent(devices[2].name);
  })

  function renderDevicesList({devices = []}: Partial<DevicesListComponent> = {}) {
    return renderFeatureComponent(DevicesListComponent, {
      imports: [DevicesModule],
      componentProperties: {devices}
    })
  }
})
