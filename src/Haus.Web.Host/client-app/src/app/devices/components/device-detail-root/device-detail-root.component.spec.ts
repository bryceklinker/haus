import {screen} from "@testing-library/dom";
import {
  eventually,
  ModelFactory,
  renderFeatureComponent,
  TestingActivatedRoute,
  TestingServer
} from "../../../../testing";
import {DeviceDetailRootComponent} from "./device-detail-root.component";
import {DevicesModule} from "../../devices.module";
import {ComponentFixture, TestBed} from "@angular/core/testing";
import {DeviceModel, DevicesService} from "../../../shared/devices";

describe('DeviceDetailRootComponent', () => {
  let device: DeviceModel;
  let activatedRoute: TestingActivatedRoute;
  let fixture: ComponentFixture<DeviceDetailRootComponent>;

  beforeEach(async () => {
    device = ModelFactory.createDeviceModel();
    TestingServer.setupGet('/api/devices', ModelFactory.createListResult(device));

    const result = await renderRoot();
    TestBed.inject(DevicesService).getAll().subscribe();

    activatedRoute = result.activatedRoute;
    fixture = result.fixture;
  })

  it('should device detail when rendered', async () => {
    activatedRoute.triggerParamsChange({deviceId: `${device.id}`});

    await eventually(() => {
      fixture.detectChanges();
      expect(screen.getByTestId('device-detail')).toHaveTextContent(device.name);
    })
  })

  function renderRoot() {
    return renderFeatureComponent(DeviceDetailRootComponent, {
      imports: [DevicesModule]
    })
  }
})
