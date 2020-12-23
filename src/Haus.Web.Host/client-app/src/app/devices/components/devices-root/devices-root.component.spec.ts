import {screen} from "@testing-library/dom";
import {eventually, ModelFactory, renderFeatureComponent, TestingServer} from "../../../../testing";
import {DevicesRootComponent} from "./devices-root.component";
import {DevicesModule} from "../../devices.module";
import {HttpMethod} from "../../../shared/rest-api";

describe('DevicesRootComponent', () => {
  it('should get all devices when rendered', async () => {
    TestingServer.setupDevicesEndpoints()

    await renderRoot();

    expect(TestingServer.lastRequest.url).toContain('/api/devices');
    expect(TestingServer.lastRequest.method).toEqual(HttpMethod.GET);
  })

  it('should show all devices', async () => {
    const devices = ModelFactory.createListResult(
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel()
    );
    TestingServer.setupGet('/api/devices', devices);

    const {detectChanges} = await renderRoot();

    await eventually(() => {
      detectChanges();
      expect(screen.queryAllByTestId('device-item')).toHaveLength(3);
    });
  })

  function renderRoot() {
    return renderFeatureComponent(DevicesRootComponent, {
      imports: [DevicesModule]
    })
  }
})
