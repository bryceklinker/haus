import {ModelFactory} from "../../../../testing";
import {DevicesListComponent} from "./devices-list.component";
import {DevicesListHarness} from "./devices-list.harness";

describe('DevicesListComponent', () => {
  it('should show each device name', async () => {
    const devices = [
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel()
    ];

    const harness = await DevicesListHarness.render({devices});

    expect(harness.deviceItems).toHaveLength(3);
    expect(harness.container).toHaveTextContent(devices[0].name);
    expect(harness.container).toHaveTextContent(devices[1].name);
    expect(harness.container).toHaveTextContent(devices[2].name);
  })
})
