import {DiscoveryUnassignedDevicesComponent} from "./discovery-unassigned-devices.component";
import {ModelFactory} from "../../../../testing";
import {DiscoveryUnassignedDevicesHarness} from "./discovery-unassigned-devices.harness";

describe('DiscoveryUnassignedDevicesComponent', () => {
  it('should show each device', async () => {
    const devices = [
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel(),
    ]

    const harness = await DiscoveryUnassignedDevicesHarness.render({devices});

    expect(harness.unassignedDevices).toHaveLength(3);
    expect(harness.container).toHaveTextContent(devices[0].name);
    expect(harness.container).toHaveTextContent(devices[1].name);
    expect(harness.container).toHaveTextContent(devices[2].name);
  })

  it('should allow each device to be dragged', async () => {
    const devices = [
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel()
    ]
    const harness = await DiscoveryUnassignedDevicesHarness.render({devices});

    expect(harness.draggableDevices).toHaveLength(3);
  })

  it('should have drop list', async () => {
    const harness = await DiscoveryUnassignedDevicesHarness.render({roomIds: [1,2,4]});

    expect(harness.unassignedDevicesZone).toBeInTheDocument();
    expect(harness.zonesUnassignedDevicesIsConnectedTo).toEqual('1,2,4')
  })
})
