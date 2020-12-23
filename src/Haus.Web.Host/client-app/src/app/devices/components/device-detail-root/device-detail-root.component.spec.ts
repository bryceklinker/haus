import {screen} from "@testing-library/dom";
import {eventually, ModelFactory, renderFeatureComponent, TestingServer} from "../../../../testing";
import {DeviceDetailRootComponent} from "./device-detail-root.component";
import {DevicesModule} from "../../devices.module";
import {TestBed} from "@angular/core/testing";
import {DevicesService} from "../../../shared/devices";

describe('DeviceDetailRootComponent', () => {
  it('should device detail when rendered', async () => {
    const device = ModelFactory.createDeviceModel();
    TestingServer.setupGet('/api/devices', ModelFactory.createListResult(device));

    const {activatedRoute, detectChanges} = await renderRoot();
    TestBed.inject(DevicesService).getAll();
    activatedRoute.triggerParamsChange({deviceId: `${device.id}`});

    await eventually(() => {
      detectChanges();
      expect(screen.getByTestId('device-detail')).toHaveTextContent(device.name);
    })
  })

  function renderRoot() {
    return renderFeatureComponent(DeviceDetailRootComponent, {
      imports: [DevicesModule]
    })
  }
})
